using Microsoft.AspNetCore.Identity;

namespace Medicare.DAL.Models;

public class User : IdentityUser<Guid>
{
    public bool IsPatient { get; set; }
    public string? Avatar { get; set; }
    public string? Description { get; set; }
    public string? FullName { get; set; }
    public List<UserSpecialization> UserSpecializations { get; set; } = new List<UserSpecialization>();
}