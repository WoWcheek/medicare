namespace Medicare.BLL.DTO;

public class AvailableDateTimeDTO
{
    public DateTime Date { get; set; }
    public List<bool> Available { get; set; } = Enumerable.Repeat(false, 16).ToList();
}
