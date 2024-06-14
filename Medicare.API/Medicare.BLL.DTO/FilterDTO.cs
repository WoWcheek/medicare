namespace Medicare.BLL.DTO;

public class FilterDTO
{
    public int? Page { get; set; }
    public List<Guid>? Specializations { get; set; }
}
