using Medicare.BLL.DTO;

namespace Medicare.BLL.Interfaces;

public interface IAppointmentManager
{
    List<AppointmentDTO> GetAppointmentsForUser(Guid id);

    List<AvailableDateTimeDTO> GetAvailableDateTimesForDoctor(Guid id);

    Task<bool> MakeAppointment(MakeAppointmentDTO makeAppointmentDTO);
}
