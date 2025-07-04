namespace All4GYM.Dtos;

public class BookingDto
{
    public int Id { get; set; }
    public int GroupSessionId { get; set; }
    public string GroupSessionTitle { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public int Duration { get; set; }
}