using Medicare.DAL.Models;

namespace Medicare.BLL.DTO;

public class AppointmentDTO
{
    public Appointment Appointment { get; set; } = null!;
    public List<string?> Specializations { get; set; } = null!;
}
