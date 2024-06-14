namespace Medicare.WebApp.Server.Requests
{
    public class AuthRegisterRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public bool IsPatient { get; set; }
    }
}
