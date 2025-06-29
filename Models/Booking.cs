namespace All4GYM.Models;

public class Booking
{
    public int Id { get; set; }

    public int SessionId { get; set; }
    public GroupSession Session { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime BookingDate { get; set; }
}