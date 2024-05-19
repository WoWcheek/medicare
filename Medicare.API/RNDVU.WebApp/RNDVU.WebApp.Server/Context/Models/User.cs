using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Medicare.Domain.Common.Models;

public class User : IdentityUser<Guid>
{
    public bool IsPatient { get; set; }
    public string? Avatar { get; set; }
    public string? Description { get; set; }
    public string? FullName { get; set; }
    public List<UserSpecialization> UserSpecializations { get; set; } = new List<UserSpecialization>();
}

public class Specialization
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public List<UserSpecialization> UserSpecializations { get; set; } = new List<UserSpecialization>();
}

public class UserSpecialization
{
    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid SpecializationId { get; set; }
    public Specialization Specialization { get; set; }
}