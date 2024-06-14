using Microsoft.AspNetCore.Mvc;
using Medicare.WebApp.Server.Requests;
using Medicare.BLL.Interfaces;
using Medicare.BLL.DTO;

namespace Medicare.WebApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] AuthRegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest("FullName, Email and Password should be filled!");
            }

            var registerDto = new RegisterDTO
            {
                Email = request.Email,
                Password = request.Password,
                FullName = request.FullName,
                IsPatient = request.IsPatient
            };

            var isRegistered = await _authManager.Register(registerDto);

            if (!isRegistered)
            {
                return BadRequest("FullName, Email and Password should be filled!");
            }

            var loginDto = new LoginDTO
            {
                Email = request.Email,
                Password = request.Password
            };

            var loggedInUser = await _authManager.Login(loginDto);

            if (loggedInUser == null)
            {
                return BadRequest("Wrong credentials");
            }

            return Ok(loggedInUser);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var loginDto = new LoginDTO
            {
                Email = request.Email,
                Password = request.Password
            };

            var loggedInUser = await _authManager.Login(loginDto);

            if (loggedInUser == null)
            {
                return BadRequest("Wrong credentials");
            }

            return Ok(loggedInUser);
        }
    }
}
