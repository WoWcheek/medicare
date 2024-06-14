namespace Medicare.DAL.Models;

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