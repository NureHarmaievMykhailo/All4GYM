using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace All4GYM.Services;

public class TrainingProgramService : ITrainingProgramService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public TrainingProgramService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<TrainingProgramDto>> GetAllAsync(int userId)
    {
        var programs = await _context.TrainingPrograms
            .Where(p => p.UserId == userId)
            .ToListAsync();
        return _mapper.Map<List<TrainingProgramDto>>(programs);
    }

    public async Task<TrainingProgramDto> GetByIdAsync(int id, int userId)
    {
        var program = await _context.TrainingPrograms
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId)
            ?? throw new Exception("Програма не знайдена");
        return _mapper.Map<TrainingProgramDto>(program);
    }

    public async Task<TrainingProgramDto> CreateAsync(CreateTrainingProgramDto dto, int userId)
    {
        var program = new TrainingProgram
        {
            Name = dto.Name,
            Description = dto.Description,
            UserId = userId
        };
        _context.TrainingPrograms.Add(program);
        await _context.SaveChangesAsync();
        return _mapper.Map<TrainingProgramDto>(program);
    }

    public async Task<TrainingProgramDto> UpdateAsync(int id, CreateTrainingProgramDto dto, int userId)
    {
        var program = await _context.TrainingPrograms
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId)
            ?? throw new Exception("Програма не знайдена");

        program.Name = dto.Name;
        program.Description = dto.Description;
        await _context.SaveChangesAsync();
        return _mapper.Map<TrainingProgramDto>(program);
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var program = await _context.TrainingPrograms
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId)
            ?? throw new Exception("Програма не знайдена");

        _context.TrainingPrograms.Remove(program);
        await _context.SaveChangesAsync();
    }
}
