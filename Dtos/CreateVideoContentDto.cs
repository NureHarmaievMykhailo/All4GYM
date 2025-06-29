namespace All4GYM.Dtos;

public class CreateVideoContentDto
{
    public string Title { get; set; } = null!;
    public string Url { get; set; } = null!;
    public int Duration { get; set; }
    public string? Category { get; set; }
}