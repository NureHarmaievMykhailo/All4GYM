using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace All4GYM.Services;

public class WorkoutService : IWorkoutService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public WorkoutService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<WorkoutDto>> GetAllAsync(int userId)
    {
        var workouts = await _context.Workouts
            .Where(w => w.UserId == userId)
            .ToListAsync();

        return _mapper.Map<List<WorkoutDto>>(workouts);
    }

    public async Task<WorkoutDto> GetByIdAsync(int id, int userId)
    {
        var workout = await _context.Workouts
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId)
            ?? throw new Exception("Тренування не знайдено");

        return _mapper.Map<WorkoutDto>(workout);
    }

    public async Task<WorkoutDto> CreateAsync(CreateWorkoutDto dto, int userId)
    {
        var program = await _context.TrainingPrograms
            .FirstOrDefaultAsync(p => p.Id == dto.TrainingProgramId)
            ?? throw new Exception("Програма не знайдена");

        var workout = new Workout
        {
            UserId = userId,
            TrainingProgramId = dto.TrainingProgramId,
            Date = dto.Date,
            Notes = dto.Notes
        };

        _context.Workouts.Add(workout);
        await _context.SaveChangesAsync();

        return _mapper.Map<WorkoutDto>(workout);
    }

    public async Task<WorkoutDto> UpdateAsync(int id, CreateWorkoutDto dto, int userId)
    {
        var workout = await _context.Workouts
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId)
            ?? throw new Exception("Тренування не знайдено");
        
        var programExists = await _context.TrainingPrograms
            .AnyAsync(p => p.Id == dto.TrainingProgramId);

        if (!programExists)
            throw new Exception("Програма не знайдена");

        workout.TrainingProgramId = dto.TrainingProgramId;
        workout.Date = dto.Date;
        workout.Notes = dto.Notes;
        await _context.SaveChangesAsync();

        return _mapper.Map<WorkoutDto>(workout);
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var workout = await _context.Workouts
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId)
            ?? throw new Exception("Тренування не знайдено");

        _context.Workouts.Remove(workout);
        await _context.SaveChangesAsync();
    }
}
