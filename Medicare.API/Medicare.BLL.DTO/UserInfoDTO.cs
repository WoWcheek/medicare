namespace Medicare.BLL.DTO;

public class UserInfoDTO
{
    public string Username { get; set; } = null!;
    public string? FullName { get; set; } = null!;
    public string? PhoneNumber { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public List<Guid>? Specializations { get; set; } = null!;
}