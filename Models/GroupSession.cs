namespace All4GYM.Models;

public class GroupSession
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;

    public int GymId { get; set; }
    public Gym Gym { get; set; } = null!;

    public int TrainerId { get; set; }
    public User Trainer { get; set; } = null!;

    public DateTime StartTime { get; set; }
    public int Duration { get; set; } // у хвилинах
    public int MaxParticipants { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}