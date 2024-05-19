namespace Medicare.WebApp.Server.Models.Requests
{
    public class AuthLoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
