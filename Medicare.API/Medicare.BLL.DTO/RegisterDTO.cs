namespace Medicare.BLL.DTO;

public class RegisterDTO
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public bool IsPatient { get; set; }
}
