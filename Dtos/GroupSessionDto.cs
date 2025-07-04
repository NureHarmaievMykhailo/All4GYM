namespace All4GYM.Dtos;

public class GroupSessionDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    
    public int GymId { get; set; }
    public string GymName { get; set; } = null!;
    
    public int TrainerId { get; set; }
    public string TrainerName { get; set; } = null!;
    
    public DateTime StartTime { get; set; }
    public int Duration { get; set; } // у хвилинах
    public int MaxParticipants { get; set; }
    
    public int CurrentParticipants { get; set; }
}