namespace Medicare.WebApp.Server.Requests
{
    public class EditUserRequest
    {
        public string? FullName { get; set; } = null!;
        public string? PhoneNumber { get; set; } = null!;
        public string? Description { get; set; } = null!;
        public List<Guid>? Specializations { get; set; } = null!;
    }
}
