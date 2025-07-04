namespace All4GYM.Dtos;

public class CreateGroupSessionDto
{
    public string Title { get; set; } = null!;

    public int GymId { get; set; }
    public int TrainerId { get; set; }

    public DateTime StartTime { get; set; }
    public int Duration { get; set; } // у хвилинах
    public int MaxParticipants { get; set; }
}