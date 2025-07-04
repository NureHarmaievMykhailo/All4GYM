using All4GYM.Data;
using All4GYM.Dtos;
using All4GYM.Models;
using Microsoft.EntityFrameworkCore;

namespace All4GYM.Services;

public class BookingService : IBookingService
{
    private readonly AppDbContext _context;

    public BookingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookingDto>> GetUserBookingsAsync(int userId)
    {
        return await _context.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.Session)
            .Select(b => new BookingDto
            {
                Id = b.Id,
                GroupSessionId = b.SessionId,
                GroupSessionTitle = b.Session.Title,
                StartTime = b.Session.StartTime,
                Duration = b.Session.Duration
            })
            .ToListAsync();
    }

    public async Task<BookingDto> CreateBookingAsync(CreateBookingDto dto, int userId)
    {
        var session = await _context.GroupSessions
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(s => s.Id == dto.SessionId);

        if (session == null)
            throw new Exception("Сесія не знайдена");

        if (session.Bookings.Any(b => b.UserId == userId))
            throw new Exception("Ви вже записані на цю сесію");

        if (session.Bookings.Count >= session.MaxParticipants)
            throw new Exception("Місць більше немає");

        var userSessions = await _context.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.Session)
            .Select(b => b.Session)
            .ToListAsync();

        var newStart = session.StartTime;
        var newEnd = newStart.AddMinutes(session.Duration);

        var hasConflict = userSessions.Any(existing =>
        {
            var existingStart = existing.StartTime;
            var existingEnd = existingStart.AddMinutes(existing.Duration);
            return existingStart < newEnd && newStart < existingEnd;
        });

        if (hasConflict)
            throw new Exception("Ця сесія перетинається з іншою, на яку ви вже записані");

        // Додати запис
        var booking = new Booking
        {
            SessionId = dto.SessionId,
            UserId = userId,
            BookingDate = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return new BookingDto
        {
            Id = booking.Id,
            GroupSessionId = session.Id,
            GroupSessionTitle = session.Title,
            StartTime = session.StartTime,
            Duration = session.Duration
        };
    }
    
    public async Task CancelBookingAsync(int sessionId, int userId)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.SessionId == sessionId && b.UserId == userId);

        if (booking == null)
            throw new Exception("Бронювання не знайдене");

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
    }

}
