using All4GYM.Dtos;

namespace All4GYM.Services;

public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetUserBookingsAsync(int userId);
    Task<BookingDto> CreateBookingAsync(CreateBookingDto dto, int userId);
    Task CancelBookingAsync(int sessionId, int userId);
}