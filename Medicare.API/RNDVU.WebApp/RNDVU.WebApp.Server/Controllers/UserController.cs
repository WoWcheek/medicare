using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Medicare.WebApp.Server.Requests;
using Medicare.BLL.DTO;
using Medicare.BLL.Interfaces;

namespace Medicare.WebApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userManager;

        public UserController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            var username = User?.Identity?.Name;

            var res = _userManager.GetUserByUsername(username);

            if (res == null)
            {
                return BadRequest("Wrong credentials");
            }

            res.Token = await HttpContext.GetTokenAsync("access_token");

            return Ok(res);
        }

        [Authorize]
        public IActionResult GetDoctors([FromBody] FilterRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var filter = new FilterDTO
            {
                Page = request.Page,
                Specializations = request.Specializations
            };

            var doctors = _userManager.GetDoctors(filter);

            return Ok(doctors);
        }

        [Authorize]
        public IActionResult GetDoctor(Guid id)
        {
            var doctor = _userManager.GetDoctorById(id);
            
            if (doctor == null)
            {
                return BadRequest();
            }

            return Ok(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeAvatar([FromBody] ChangeAvatarRequest request)
        {
            if (request == null || User?.Identity?.Name == null)
            {
                return BadRequest();
            }

            var avatarDto = new AvatarDTO
            {
                Username = User.Identity.Name,
                Avatar = request.Base64
            };

            bool isAvatarSet = await _userManager.ChangeAvatar(avatarDto);

            if (!isAvatarSet)
            {
                return BadRequest("Wrong credentials");
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request == null || User?.Identity?.Name == null)
            {
                return BadRequest();
            }

            var passwordDto = new PasswordDTO
            {
                Username = User.Identity.Name,
                OldPassword = request.Old,
                NewPassword = request.New
            };

            var isPasswordSet = await _userManager.ChangePassword(passwordDto);

            if (!isPasswordSet)
            {
                return BadRequest("Wrong credentials");
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> EditUser([FromBody] EditUserRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.FullName))
            {
                return BadRequest("FullName should be filled!");
            }

            if (User?.Identity?.Name == null)
            {
                return BadRequest("Wrong credentials");
            }

            var userInfoDto = new UserInfoDTO
            {
                Username = User.Identity.Name,
                FullName = request.FullName,
                Description = request.Description,
                PhoneNumber = request.PhoneNumber,
                Specializations = request.Specializations
            };

            var isUserUpdated = await _userManager.EditUserInfo(userInfoDto);

            if (!isUserUpdated)
            {
                return BadRequest("Wrong credentials");
            }

            return Ok();
        }
    }
}
