using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Medicare.WebApp.Server.Requests;
using Medicare.BLL.DTO;
using Medicare.BLL.Interfaces;

namespace Medicare.WebApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentManager _appointmentManager;

        public AppointmentController(IAppointmentManager appointmentManager)
        {
            _appointmentManager = appointmentManager;
        }

        [Authorize]
        public IActionResult GetAppointments(Guid id)
        {
            var appointments = _appointmentManager.GetAppointmentsForUser(id);
            return Ok(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> MakeAppointment([FromBody] MakeAppointmentRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var makeAppDto = new MakeAppointmentDTO
            {
                UserId = request.UserId,
                DoctorId = request.DoctorId,
                Date = request.Date,
                Time = request.Time,
                IsShort = request.IsShort,
            };

            var isCreated = await _appointmentManager.MakeAppointment(makeAppDto);
            
            if (!isCreated)
            {
                return BadRequest();
            }

            return Ok();
        }

        [Authorize]
        public IActionResult GetAvailableDoctorAppointments(Guid id)
        {
            var appointmentsAvailable = _appointmentManager
                .GetAvailableDateTimesForDoctor(id);

            return Ok(appointmentsAvailable);
        }
    }
}
