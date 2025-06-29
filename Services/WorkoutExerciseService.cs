using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Models;
using Microsoft.EntityFrameworkCore;

namespace All4GYM.Services;

public class WorkoutExerciseService : IWorkoutExerciseService
{
    private readonly AppDbContext _context;

    public WorkoutExerciseService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<WorkoutExerciseDto>> GetByWorkoutIdAsync(int workoutId, int userId)
    {
        var workout = await _context.Workouts
            .Include(w => w.TrainingProgram)
            .FirstOrDefaultAsync(w => w.Id == workoutId && w.TrainingProgram.UserId == userId)
            ?? throw new Exception("Тренування не знайдено або не належить вам");

        var items = await _context.WorkoutExercises
            .Where(we => we.WorkoutId == workoutId)
            .Include(we => we.Exercise)
            .ToListAsync();

        return items.Select(we => new WorkoutExerciseDto
        {
            WorkoutId = we.WorkoutId,
            ExerciseId = we.ExerciseId,
            ExerciseName = we.Exercise.Name,
            Sets = we.Sets,
            Reps = we.Reps,
            Weight = we.Weight
        }).ToList();
    }

    public async Task AddAsync(int workoutId, AddWorkoutExerciseDto dto, int userId)
    {
        var workout = await _context.Workouts
            .Include(w => w.TrainingProgram)
            .FirstOrDefaultAsync(w => w.Id == workoutId && w.TrainingProgram.UserId == userId)
            ?? throw new Exception("Тренування не знайдено або не належить вам");

        var exists = await _context.WorkoutExercises
            .AnyAsync(we => we.WorkoutId == workoutId && we.ExerciseId == dto.ExerciseId);

        if (exists)
            throw new Exception("Ця вправа вже додана до тренування");

        var we = new WorkoutExercise
        {
            WorkoutId = workoutId,
            ExerciseId = dto.ExerciseId,
            Sets = dto.Sets,
            Reps = dto.Reps,
            Weight = dto.Weight
        };

        _context.WorkoutExercises.Add(we);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(int workoutId, int exerciseId, int userId)
    {
        var workout = await _context.Workouts
            .Include(w => w.TrainingProgram)
            .FirstOrDefaultAsync(w => w.Id == workoutId && w.TrainingProgram.UserId == userId)
            ?? throw new Exception("Тренування не знайдено або не належить вам");

        var we = await _context.WorkoutExercises
            .FirstOrDefaultAsync(we => we.WorkoutId == workoutId && we.ExerciseId == exerciseId)
            ?? throw new Exception("Вправа не знайдена у цьому тренуванні");

        _context.WorkoutExercises.Remove(we);
        await _context.SaveChangesAsync();
    }
}
