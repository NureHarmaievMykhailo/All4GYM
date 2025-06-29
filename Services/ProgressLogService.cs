using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace All4GYM.Services;

public class ProgressLogService : IProgressLogService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ProgressLogService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ProgressLogDto>> GetAllAsync(int userId)
    {
        var logs = await _context.ProgressLogs
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.Date)
            .ToListAsync();

        return _mapper.Map<List<ProgressLogDto>>(logs);
    }

    public async Task<ProgressLogDto> GetByIdAsync(int id, int userId)
    {
        var log = await _context.ProgressLogs
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId)
            ?? throw new Exception("Запис не знайдено");

        return _mapper.Map<ProgressLogDto>(log);
    }

    public async Task<ProgressLogDto> CreateAsync(CreateProgressLogDto dto, int userId)
    {
        var log = _mapper.Map<ProgressLog>(dto);
        log.UserId = userId;

        _context.ProgressLogs.Add(log);
        await _context.SaveChangesAsync();

        return _mapper.Map<ProgressLogDto>(log);
    }

    public async Task<ProgressLogDto> UpdateAsync(int id, CreateProgressLogDto dto, int userId)
    {
        var log = await _context.ProgressLogs
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId)
            ?? throw new Exception("Запис не знайдено");

        log.Date = dto.Date;
        log.Weight = dto.Weight;
        log.MuscleMass = dto.MuscleMass;
        log.BodyFat = dto.BodyFat;
        log.Notes = dto.Notes;

        await _context.SaveChangesAsync();
        return _mapper.Map<ProgressLogDto>(log);
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var log = await _context.ProgressLogs
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId)
            ?? throw new Exception("Запис не знайдено");

        _context.ProgressLogs.Remove(log);
        await _context.SaveChangesAsync();
    }
}
