namespace All4GYM.Models;

public class Gym
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;

    public ICollection<GroupSession> GroupSessions { get; set; } = new List<GroupSession>();
}