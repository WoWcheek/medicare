using Azure.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Medicare.Domain.Common.Models;
using Medicare.WebApp.Server.Configuration;
using Medicare.WebApp.Server.Context;
using Medicare.WebApp.Server.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Principal;
using Azure;
using Microsoft.AspNetCore.DataProtection;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Medicare.WebApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private IConfiguration _conf;
        private MedicareContext _context;
        private UserManager<User> UserManager;

        public AuthController(MedicareContext context, UserManager<User> userManager, IConfiguration conf)
        {
            _context = context;
            UserManager = userManager;
            _conf = conf;
        }

        [Authorize]

        public async Task<IActionResult> GetUser()
        {
            var user = _context.Users.Include(x => x.UserSpecializations).FirstOrDefault(x => x.Email == User.Identity.Name);

            return Ok(new
            {
                user.Id,
                user.IsPatient,
                user.FullName,
                user.Description,
                user.Avatar,
                user.Email,
                user.PhoneNumber,
                token = HttpContext.GetTokenAsync("access_token"),
                specializations = user.UserSpecializations?.Select(x => x.SpecializationId).ToList()
            });
        }
        [Authorize]
        public async Task<IActionResult> GetDoctors([FromBody] FilterModel request)
        {
            var users = _context.Users.Include(x => x.UserSpecializations).ThenInclude(x => x.Specialization)
                .Where(x => !x.IsPatient)
                .ToList()
                .Where(x => request.Specializations?.Any(y => x.UserSpecializations.Any(u => u.SpecializationId == y)) ?? true)
                .Skip((request.Page ?? 0) * 9)
                .Take(9);

            return Ok(new
            {
                count = _context.Users.Include(x => x.UserSpecializations).ThenInclude(x => x.Specialization)
                .Where(x => !x.IsPatient)
                .ToList()
                .Where(x => request.Specializations?.Any(y => x.UserSpecializations.Any(u => u.SpecializationId == y)) ?? true).Count(),
                doctors = users.Select(user => new
                {
                    user.Id,
                    user.FullName,
                    user.Description,
                    user.Avatar,
                    user.Email,
                    user.PhoneNumber,
                    specializations = user.UserSpecializations?.Select(x => x.Specialization.Name).ToList()
                })
            });
        }
        [Authorize]
        public async Task<IActionResult> GetDoctor(Guid id)
        {
            var user = _context.Users.Include(x => x.UserSpecializations).ThenInclude(x => x.Specialization).FirstOrDefault(x => x.Id == id);

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Description,
                user.Avatar,
                user.Email,
                user.PhoneNumber,
                specializations = user.UserSpecializations?.Select(x => x.Specialization.Name).ToList()
            });
        }
        [Authorize]
        public async Task<IActionResult> GetAppointments(Guid id)
        {
            var appointments = _context.Appointments.AsSplitQuery().Include(x=>x.Doctor).Include(x=>x.User).Where(x =>  (x.DoctorId == id || x.UserId == id)).ToList().Where(x =>
            {
               var t =  new DateTime(x.Date.Year, x.Date.Month, x.Date.Day).AddHours(Convert.ToInt64(x.Time.Substring(0, 2)));

                if (x.Time[3] == '3')
                {
                    t.AddMinutes(30);
                }
                return t > DateTime.Now;
            }).ToList();

            var dgs = appointments.Select(y =>
            {
                return new
                {
                    appointment = y,
                    specializations = _context.Users
                    .AsNoTracking()
                        .Include(x => x.UserSpecializations)
                        .ThenInclude(x => x.Specialization)
                        .First(t => t.Id == y.DoctorId)
                        .UserSpecializations
                        .Select(t => t.Specialization.Name)
                        .ToList()
                };
            });


            return Ok(dgs);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] AuthRegisterRequest request)
        {
            if (request == null
                || string.IsNullOrEmpty(request.FullName)
                || string.IsNullOrEmpty(request.Email)
                || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("FullName, Email, Password Should be filled!");
            }

            try
            {
                var sha1data = new PasswordHasher<User>()
                    .HashPassword(null!, request.Password);

                await _context.Users
                    .AddAsync(new User
                    {
                        Email = request.Email,
                        FullName = request.FullName,
                        UserName = request.Email,
                        IsPatient = request.IsPatient,
                        PasswordHash = sha1data,
                    });

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return await Login(request.Email, request.Password);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeAvatar([FromBody] ChangeAvatarRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            try
            {
                var user = _context.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
                user.Avatar = request.Base64;
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw;
            }

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> MakeAppointment([FromBody] Appointment request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            try
            {
                request.Id = Guid.NewGuid();
                request.Url = await CreateMeeting(request.IsShort, request.Date, request.Time);
                _context.Appointments.Add(request);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw;
            }

            return Ok();
        }

        class rew
        {
            public DateTime Date { get; set; }
            public List<bool> Available { get; set; } = new() { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
        }

        [Authorize]
        public async Task<IActionResult> GetDoctorAppoimtments(Guid id)
        {
            var user = _context.Appointments.Where(x => x.Date > DateTime.Today && x.DoctorId == id);
            var t = user.GroupBy(x => x.Date);
            var timesIndexes = new Dictionary<string, int>(){
               {"10:00", 0 },{"10:30",1},{"11:00",2},{"11:30",3},{"12:00",4},{"12:30",5},{"13:00",6},{"13:30",7},{"14:00",8},{"14:30",9},{"15:00",10},{"15:30",11},{"16:00",12},{"16:30",13},{"17:00",14}, {"17:30",15}
            };

            List<rew> res = new();
            foreach (var i in t)
            {
                var r = new rew() { Date = i.Key };
                foreach (var item in i.OrderBy(x => x.Time))
                {
                    r.Available[timesIndexes[item.Time]] = true;
                    if (!item.IsShort)
                        r.Available[timesIndexes[item.Time] + 1] = true;
                }
                res.Add(r);
            }

            return Ok(res);
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            try
            {
                var use = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id;

                var user = await UserManager.FindByIdAsync(use.ToString());

                var sha1data = await UserManager.ChangePasswordAsync(user, request.Old, request.New);

                if (!sha1data.Succeeded)
                {
                    return BadRequest("wrong creds");
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw;
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> EditUser([FromBody] EditUserRequest request)
        {
            if (request == null
                || string.IsNullOrEmpty(request.FullName))
            {
                return BadRequest("FullName should be filled!");
            }


            try
            {
                var user = _context.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
                user.FullName = request.FullName;
                user.Description = request.Description;
                user.PhoneNumber = request.PhoneNumber;

                _context.RemoveRange(_context.UserSpecializations.Where(x => x.UserId == user.Id).ToList());
                _context.UserSpecializations.AddRange(request.Specializations.Select(x => new UserSpecialization() { UserId = user.Id, SpecializationId = x }));

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw;
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            return await Login(request.Email, request.Password);
        }

        [NonAction]
        private async Task<IActionResult> Login(string Email, string Password)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                return BadRequest();
            }

            string token = string.Empty;
            Guid? id = null;
            bool isPatient = false;

            try
            {
                var user = await _context.Users
                     .FirstOrDefaultAsync(x =>
                         x.Email == Email);

                if (user == null || user.PasswordHash == null)
                {
                    return BadRequest("Wrong creds");
                }


                var sha1data = new PasswordHasher<User>()
                    .VerifyHashedPassword(null!, user.PasswordHash, Password);

                if (sha1data == PasswordVerificationResult.Failed)
                {
                    return BadRequest("Wrong creds");
                }

                ClaimsIdentity claim = new ClaimsIdentity(
                    new List<Claim>()
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email)
                    },
                    "Token",
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                var now = DateTime.Now;
                var securityToken = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: claim.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

                var resp = new JwtSecurityTokenHandler()
                    .WriteToken(securityToken);

                token = resp;
                id = user.Id;
                isPatient = user.IsPatient;
            }
            catch (Exception e)
            {
                throw;
            }

            return Ok(new { token, isPatient, id });
        }


        public async Task<string> CreateMeeting(bool isShort, DateTime date, string time)
        {

            DateTime tof = new DateTime(date.Year, date.Month, date.Day).AddHours(Convert.ToInt64(time.Substring(0, 2)));
           
            if (time[3] == '3')
            {
                tof.AddMinutes(30);
            }

            string meetingTopic = "Medicare";
            string meetingStartTime = tof.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            int meetingDuration = isShort ? 30: 60;
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // Generate JWT Token
            var jwtToken = await GenerateJWTToken();
            // Create Zoom Meeting
            string apiUrl = "https://api.zoom.us/v2/users/me/meetings";
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var requestBody = new
            {
                topic = meetingTopic,
                type = 2,
                start_time = meetingStartTime,
                duration = meetingDuration,
                timezone = "UTC",
                settings = new
                {
                    host_video = true,
                    participant_video = true,
                    join_before_host = true
                }
            };
            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(apiUrl, requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<dfg>(responseContent).join_url;
        }
        private async Task<string> GenerateJWTToken()
        {

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var str = _conf.GetValue<string>("Zoom:ClientId") + ":" + _conf.GetValue<string>("Zoom:ClientSecret");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(str)));

            var requestContent = new StringContent("account_id=" + _conf.GetValue<string>("Zoom:AccountId") + "&grant_type=account_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await httpClient.PostAsync("https://zoom.us/oauth/token", requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<dfg>(responseContent).access_token;
        }
        class dfg
        {
            public string access_token {get;set;}
            public string join_url {get; set; }
        }

        private static string Base64UrlEncode(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            string base64String = Convert.ToBase64String(inputBytes);
            return base64String.TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }
        private static string ComputeHMACSHA256(string input, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            using (var hmac = new System.Security.Cryptography.HMACSHA256(keyBytes))
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = hmac.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
            }
        }
    }
}
