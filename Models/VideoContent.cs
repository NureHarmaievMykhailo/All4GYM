namespace All4GYM.Models;

public class VideoContent
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Url { get; set; } = null!;
    public int Duration { get; set; } // у секундах
    public string? Category { get; set; }
}