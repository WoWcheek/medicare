using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Medicare.Domain.Common.Models;
using Medicare.WebApp.Server.Context;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace Medicare.WebApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AppointmentController : ControllerBase
    {
        private IConfiguration _conf;
        private MedicareContext _context;
        private UserManager<User> UserManager;

        public AppointmentController(MedicareContext context, UserManager<User> userManager, IConfiguration conf)
        {
            _context = context;
            UserManager = userManager;
            _conf = conf;
        }

        [Authorize]
        public async Task<IActionResult> GetAppointments(Guid id)
        {
            var appointments = _context.Appointments.AsSplitQuery().Include(x => x.Doctor).Include(x => x.User).Where(x => (x.DoctorId == id || x.UserId == id)).ToList().Where(x =>
            {
                var t = new DateTime(x.Date.Year, x.Date.Month, x.Date.Day).AddHours(Convert.ToInt64(x.Time.Substring(0, 2)));

                if (x.Time[3] == '3')
                {
                    t.AddMinutes(30);
                }
                return t > DateTime.Now;
            }).ToList();

            var dgs = appointments.OrderBy(x => x.Date).ThenBy(x => x.Time).Select(y =>
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
        public async Task<IActionResult> MakeAppointment([FromBody] Appointment request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            try
            {
                request.Date = request.Date.ToLocalTime();
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
        public async Task<IActionResult> GetDoctorAppointments(Guid id)
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

        public async Task<string> CreateMeeting(bool isShort, DateTime date, string time)
        {
            DateTime tof = new DateTime(date.Year, date.Month, date.Day).AddHours(Convert.ToInt64(time.Substring(0, 2)));

            if (time[3] == '3')
            {
                tof.AddMinutes(30);
            }

            string meetingTopic = "Medicare";
            string meetingStartTime = tof.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            int meetingDuration = isShort ? 30 : 60;
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var jwtToken = await GenerateJWTToken();
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
            public string access_token { get; set; }
            public string join_url { get; set; }
        }
    }
}
