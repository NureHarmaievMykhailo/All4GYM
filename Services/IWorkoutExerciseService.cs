using All4GYM.Dtos;

namespace All4GYM.Services;

public interface IWorkoutExerciseService
{
    Task<List<WorkoutExerciseDto>> GetByWorkoutIdAsync(int workoutId, int userId);
    Task AddAsync(int workoutId, AddWorkoutExerciseDto dto, int userId);
    Task RemoveAsync(int workoutId, int exerciseId, int userId);
}