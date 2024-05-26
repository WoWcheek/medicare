namespace Medicare.WebApp.Server.Models.Requests
{
    public class MakeAppointmentRequest
    {
        public DateTime Date { get; set; } 
        public string Time { get; set; } = null!;
        public bool IsShort { get; set; } 
        public Guid UserId { get; set; } 
        public Guid DoctorId { get; set; }
    }
}
