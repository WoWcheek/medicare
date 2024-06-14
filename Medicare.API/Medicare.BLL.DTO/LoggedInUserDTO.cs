namespace Medicare.BLL.DTO;

public class LoggedInUserDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public bool IsPatient { get; set; }
}
