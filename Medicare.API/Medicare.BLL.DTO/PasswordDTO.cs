namespace Medicare.BLL.DTO;

public class PasswordDTO
{
    public string Username { get; set; } = null!;
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
