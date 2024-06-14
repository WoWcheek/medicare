namespace Medicare.WebApp.Server.Requests
{
    public class MakeAppointmentRequest
    {
        public Guid UserId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; } = null!;
        public bool IsShort { get; set; }
    }
}
