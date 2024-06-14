namespace Medicare.WebApp.Server.Requests
{
    public class AuthLoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
