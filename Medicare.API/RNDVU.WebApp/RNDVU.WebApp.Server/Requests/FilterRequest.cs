namespace Medicare.WebApp.Server.Requests
{
    public class FilterRequest
    {
        public int? Page { get; set; }
        public List<Guid>? Specializations { get; set; }
    }
}
