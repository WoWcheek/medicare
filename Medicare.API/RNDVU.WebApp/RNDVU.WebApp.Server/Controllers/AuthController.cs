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

namespace Medicare.WebApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private MedicareContext _context;
        private UserManager<User> UserManager;

        public AuthController(MedicareContext context, UserManager<User> userManager)
        {
            _context = context;
            UserManager = userManager;
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
            var users = _context.Users.Include(x => x.UserSpecializations).ThenInclude(x=>x.Specialization)
                .Where(x => !x.IsPatient)
                .ToList()
                .Where(x => request.Specializations?.Any(y => x.UserSpecializations.Any(u => u.SpecializationId == y)) ?? true)
                .Skip((request.Page ?? 0) * 9)
                .Take(9);

            return Ok(new { count = _context.Users.Include(x => x.UserSpecializations).ThenInclude(x => x.Specialization)
                .Where(x => !x.IsPatient)
                .ToList()
                .Where(x => request.Specializations?.Any(y => x.UserSpecializations.Any(u => u.SpecializationId == y)) ?? true).Count(), doctors= users.Select(user => new {
                user.Id,
                user.FullName,
                user.Description,
                user.Avatar,
                user.Email,
                user.PhoneNumber,
                specializations = user.UserSpecializations?.Select(x => x.Specialization.Name).ToList()
            })});
        }
        [Authorize]
        public async Task<IActionResult> GetDoctor(Guid id)
        {
            var user = _context.Users.Include(x => x.UserSpecializations).FirstOrDefault(x => x.Id == id);

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Description,
                user.Avatar,
                user.Email,
                user.PhoneNumber,
                specializations = user.UserSpecializations?.Select(x => x.SpecializationId).ToList()
            });
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
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            try
            {
                var use  = _context.Users.AsNoTracking().FirstOrDefault(x => x.Email == User.Identity.Name).Id;
               
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
                || string.IsNullOrEmpty(request.FullName) )
            {
                return BadRequest("FullName should be filled!");
            }


            try
            {
                var user = _context.Users.FirstOrDefault(x => x.Email == User.Identity.Name);
                user.FullName = request.FullName;
                user.Description = request.Description;
                user.PhoneNumber = request.PhoneNumber;
                
                _context.RemoveRange( _context.UserSpecializations.Where(x => x.UserId == user.Id).ToList());
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

            return Ok(new { token, isPatient,  id });
        }
    }
}
