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

public class Appointment
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string Time { get; set; } = null!;
    public string? Url { get; set; } = null!;
    public bool IsShort { get; set; }

    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public Guid? DoctorId { get; set; }
    public User? Doctor { get; set; }
}