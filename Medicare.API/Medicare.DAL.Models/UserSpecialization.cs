namespace Medicare.DAL.Models;

public class UserSpecialization
{
    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid SpecializationId { get; set; }
    public Specialization Specialization { get; set; }
}
