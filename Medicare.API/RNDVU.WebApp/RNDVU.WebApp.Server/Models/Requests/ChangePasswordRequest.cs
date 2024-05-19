namespace Medicare.WebApp.Server.Models.Requests
{
    public class ChangePasswordRequest
    {
        public string Old { get; set; } = null!;
        public string New { get; set; } = null!;
    }
}
