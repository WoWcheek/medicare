namespace Medicare.BLL.DTO;

public class MeetingDTO
{
    public bool IsShort { get; set; }
    public DateTime Date { get; set; }
    public string Time { get; set; } = null!;
}
