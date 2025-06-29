using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace All4GYM.Services;

public class VideoContentService : IVideoContentService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public VideoContentService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<VideoContentDto>> GetAllAsync()
    {
        var videos = await _context.VideoContents.ToListAsync();
        return _mapper.Map<List<VideoContentDto>>(videos);
    }

    public async Task<VideoContentDto> GetByIdAsync(int id)
    {
        var video = await _context.VideoContents.FindAsync(id)
                    ?? throw new Exception("Відео не знайдено");

        return _mapper.Map<VideoContentDto>(video);
    }

    public async Task<VideoContentDto> CreateAsync(CreateVideoContentDto dto)
    {
        var video = _mapper.Map<VideoContent>(dto);
        _context.VideoContents.Add(video);
        await _context.SaveChangesAsync();
        return _mapper.Map<VideoContentDto>(video);
    }

    public async Task<VideoContentDto> UpdateAsync(int id, CreateVideoContentDto dto)
    {
        var video = await _context.VideoContents.FindAsync(id)
                    ?? throw new Exception("Відео не знайдено");

        video.Title = dto.Title;
        video.Url = dto.Url;
        video.Duration = dto.Duration;
        video.Category = dto.Category;

        await _context.SaveChangesAsync();
        return _mapper.Map<VideoContentDto>(video);
    }

    public async Task DeleteAsync(int id)
    {
        var video = await _context.VideoContents.FindAsync(id)
                    ?? throw new Exception("Відео не знайдено");

        _context.VideoContents.Remove(video);
        await _context.SaveChangesAsync();
    }
}