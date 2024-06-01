using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Medicare.Domain.Common.Models;
using Medicare.WebApp.Server.Context;
using Medicare.WebApp.Server.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Globalization;

namespace Medicare.WebApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private MedicareContext _context;
        private UserManager<User> UserManager;

        public UserController(MedicareContext context, UserManager<User> userManager)
        {
            _context = context;
            UserManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            var user = _context.Users.Include(x => x.UserSpecializations).FirstOrDefault(x => x.Email == User.Identity.Name);
            var allAppoinementsCount = _context.Appointments.Where(x => (x.DoctorId == user.Id || x.UserId == user.Id)).Count();
            var upcomingCount = _context.Appointments.Where(x => (x.DoctorId == user.Id || x.UserId == user.Id)).ToList().Where(x =>
            {
                var t = new DateTime(x.Date.Year, x.Date.Month, x.Date.Day).AddHours(Convert.ToInt64(x.Time.Substring(0, 2)));

                if (x.Time[3] == '3')
                {
                    t.AddMinutes(30);
                }
                return t > DateTime.Now;
            }).Count();

            var differentDoctorsCount = _context.Appointments
                .Where(x => (x.DoctorId == user.Id || x.UserId == user.Id)).Select(x => x.DoctorId).Distinct().Count();

            var differentUsersCount = _context.Appointments
                .Where(x => (x.DoctorId == user.Id || x.UserId == user.Id)).Select(x => x.UserId).Distinct().Count();

            var weekAppointmentsCount = _context.Appointments
                .Where(x => (x.DoctorId == user.Id || x.UserId == user.Id)).ToList().Where(x =>
                {
                    DateTime now = DateTime.Now;

                    CultureInfo cultureInfo = CultureInfo.CurrentCulture;
                    DayOfWeek firstDayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;
                    DateTime startOfWeek = now.Date;
                    while (startOfWeek.DayOfWeek != firstDayOfWeek)
                    {
                        startOfWeek = startOfWeek.AddDays(-1);
                    }
                    DateTime endOfWeek = startOfWeek.AddDays(6);
                    return x.Date.Date >= startOfWeek && x.Date.Date <= endOfWeek;
                });

            var dayAppointmentsCount = _context.Appointments
                .Where(x => (x.DoctorId == user.Id || x.UserId == user.Id)).ToList().Where(x =>
                {
                    return x.Date.Date == DateTime.Now.Date;
                });

            return Ok(new
            {
                user.Id,
                user.IsPatient,
                user.FullName,
                user.Description,
                user.Avatar,
                user.Email,
                user.PhoneNumber,
                allAppoinementsCount,
                upcomingCount,
                differentDoctorsOrUsersCount = user.IsPatient ? differentDoctorsCount : differentUsersCount,
                weekAppointmentsCount,
                dayAppointmentsCount,
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
    }
}
