using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Models;
using Microsoft.EntityFrameworkCore;

namespace All4GYM.Services;

public class GroupSessionService : IGroupSessionService
{
    private readonly AppDbContext _context;

    public GroupSessionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<GroupSessionDto>> GetAllAsync()
    {
        var sessions = await _context.GroupSessions
            .Include(s => s.Gym)
            .Include(s => s.Trainer)
            .Include(s => s.Bookings)
            .ToListAsync();

        return sessions.Select(s => new GroupSessionDto
        {
            Id = s.Id,
            Title = s.Title,
            GymId = s.GymId,
            GymName = s.Gym.Name,
            TrainerId = s.TrainerId,
            TrainerName = s.Trainer.FullName,
            StartTime = s.StartTime,
            Duration = s.Duration,
            MaxParticipants = s.MaxParticipants,
            CurrentParticipants = s.Bookings.Count
        }).ToList();
    }

    public async Task<GroupSessionDto> GetByIdAsync(int id)
    {
        var s = await _context.GroupSessions
            .Include(s => s.Gym)
            .Include(s => s.Trainer)
            .FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new Exception("Сесія не знайдена");

        return new GroupSessionDto
        {
            Id = s.Id,
            Title = s.Title,
            GymId = s.GymId,
            GymName = s.Gym.Name,
            TrainerId = s.TrainerId,
            TrainerName = s.Trainer.FullName,
            StartTime = s.StartTime,
            Duration = s.Duration,
            MaxParticipants = s.MaxParticipants
        };
    }

    public async Task<GroupSessionDto> CreateAsync(CreateGroupSessionDto dto)
    {
        var session = new GroupSession
        {
            Title = dto.Title,
            GymId = dto.GymId,
            TrainerId = dto.TrainerId,
            StartTime = dto.StartTime,
            Duration = dto.Duration,
            MaxParticipants = dto.MaxParticipants
        };

        _context.GroupSessions.Add(session);
        await _context.SaveChangesAsync();

        // Повертаємо DTO з GymName і TrainerName
        var gym = await _context.Gyms.FindAsync(dto.GymId);
        var trainer = await _context.Users.FindAsync(dto.TrainerId);

        return new GroupSessionDto
        {
            Id = session.Id,
            Title = session.Title,
            GymId = session.GymId,
            GymName = gym?.Name ?? "N/A",
            TrainerId = session.TrainerId,
            TrainerName = trainer?.FullName ?? "N/A",
            StartTime = session.StartTime,
            Duration = session.Duration,
            MaxParticipants = session.MaxParticipants
        };
    }

    public async Task<GroupSessionDto> UpdateAsync(int id, CreateGroupSessionDto dto)
    {
        var session = await _context.GroupSessions.FindAsync(id)
            ?? throw new Exception("Сесія не знайдена");

        session.Title = dto.Title;
        session.GymId = dto.GymId;
        session.TrainerId = dto.TrainerId;
        session.StartTime = dto.StartTime;
        session.Duration = dto.Duration;
        session.MaxParticipants = dto.MaxParticipants;

        await _context.SaveChangesAsync();

        var gym = await _context.Gyms.FindAsync(dto.GymId);
        var trainer = await _context.Users.FindAsync(dto.TrainerId);

        return new GroupSessionDto
        {
            Id = session.Id,
            Title = session.Title,
            GymId = session.GymId,
            GymName = gym?.Name ?? "N/A",
            TrainerId = session.TrainerId,
            TrainerName = trainer?.FullName ?? "N/A",
            StartTime = session.StartTime,
            Duration = session.Duration,
            MaxParticipants = session.MaxParticipants
        };
    }

    public async Task DeleteAsync(int id)
    {
        var session = await _context.GroupSessions.FindAsync(id)
            ?? throw new Exception("Сесія не знайдена");

        _context.GroupSessions.Remove(session);
        await _context.SaveChangesAsync();
    }
}
