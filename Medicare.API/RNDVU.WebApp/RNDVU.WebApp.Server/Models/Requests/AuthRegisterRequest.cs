namespace Medicare.WebApp.Server.Models.Requests
{
    public class AuthRegisterRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public bool IsPatient { get; set; }
    }
    public class FilterModel
    {
        public int? Page { get; set; } = null!;
        public List<Guid>? Specializations { get; set; }
    }
}
