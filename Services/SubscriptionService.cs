using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace All4GYM.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public SubscriptionService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<SubscriptionDto>> GetAllAsync(int userId)
    {
        var subs = await _context.Subscriptions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.StartDate)
            .ToListAsync();

        return _mapper.Map<List<SubscriptionDto>>(subs);
    }

    public async Task<SubscriptionDto> GetByIdAsync(int id, int userId)
    {
        var sub = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId)
            ?? throw new Exception("Підписку не знайдено");

        return _mapper.Map<SubscriptionDto>(sub);
    }

    public async Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto dto, int userId)
    {
        var sub = _mapper.Map<Subscription>(dto);
        sub.UserId = userId;

        _context.Subscriptions.Add(sub);
        await _context.SaveChangesAsync();

        return _mapper.Map<SubscriptionDto>(sub);
    }

    public async Task<SubscriptionDto> UpdateAsync(int id, CreateSubscriptionDto dto, int userId)
    {
        var sub = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId)
            ?? throw new Exception("Підписку не знайдено");

        sub.Type = dto.Type;
        sub.StartDate = dto.StartDate;
        sub.EndDate = dto.EndDate;
        sub.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
        return _mapper.Map<SubscriptionDto>(sub);
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var sub = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId)
            ?? throw new Exception("Підписку не знайдено");

        _context.Subscriptions.Remove(sub);
        await _context.SaveChangesAsync();
    }
    
    public async Task CancelAsync(int userId)
    {
        var activeSubs = await _context.Subscriptions
            .Where(s => s.UserId == userId && s.IsActive)
            .ToListAsync();

        if (!activeSubs.Any())
            throw new Exception("Активна підписка не знайдена.");

        foreach (var sub in activeSubs)
            sub.IsActive = false;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is not null)
        {
            user.HasActiveSubscription = false;
            user.SubscriptionTier = SubscriptionTier.Basic;
        }

        await _context.SaveChangesAsync();
    }

}
