namespace Medicare.BLL.DTO
{
    public class LoggedInUserWithTokenDTO
    {
        public Guid Id { get; set; }
        public bool IsPatient { get; set; }
        public string Token { get; set; } = null!;
    }
}
