namespace All4GYM.Dtos;

public class SubscriptionDto
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
}