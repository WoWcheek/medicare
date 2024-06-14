namespace Medicare.BLL.DTO;

public class DoctorsDTO
{
    public int Count { get; set; }
    public List<DoctorDTO> Doctors { get; set; } = null!;
}
