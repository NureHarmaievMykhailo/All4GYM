using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace All4GYM.Services;

public class ExerciseService : IExerciseService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ExerciseService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ExerciseDto>> GetAllAsync()
    {
        var exercises = await _context.Exercises.ToListAsync();
        return _mapper.Map<List<ExerciseDto>>(exercises);
    }

    public async Task<ExerciseDto> GetByIdAsync(int id)
    {
        var exercise = await _context.Exercises.FindAsync(id)
                       ?? throw new Exception("Вправу не знайдено");

        return _mapper.Map<ExerciseDto>(exercise);
    }

    public async Task<ExerciseDto> CreateAsync(CreateExerciseDto dto)
    {
        var exercise = _mapper.Map<Exercise>(dto);
        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();
        return _mapper.Map<ExerciseDto>(exercise);
    }

    public async Task<ExerciseDto> UpdateAsync(int id, CreateExerciseDto dto)
    {
        var exercise = await _context.Exercises.FindAsync(id)
                       ?? throw new Exception("Вправу не знайдено");

        exercise.Name = dto.Name;
        exercise.Description = dto.Description;
        exercise.MuscleGroup = dto.MuscleGroup;

        await _context.SaveChangesAsync();
        return _mapper.Map<ExerciseDto>(exercise);
    }

    public async Task DeleteAsync(int id)
    {
        var exercise = await _context.Exercises.FindAsync(id)
                       ?? throw new Exception("Вправу не знайдено");

        _context.Exercises.Remove(exercise);
        await _context.SaveChangesAsync();
    }
}