namespace Medicare.BLL.DTO;

public class DoctorDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Avatar { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public List<string?> Specializations { get; set; } = null!;
}
