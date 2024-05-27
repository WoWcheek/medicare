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

namespace Medicare.WebApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private MedicareContext _context;

        public AuthController(MedicareContext context)
        {
            _context = context;
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
    }
}
