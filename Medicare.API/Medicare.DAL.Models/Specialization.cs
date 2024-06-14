namespace Medicare.DAL.Models;

public class Specialization
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public List<UserSpecialization> UserSpecializations { get; set; } = new List<UserSpecialization>();
}
