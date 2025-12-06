using Bookify.Data.Models;

namespace Bookify.services.IServices
{
    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<Booking> GetByIdAsync(int id);
        Task<Booking> CreateBookingAsync(Booking booking);
        Task<Booking> UpdateBookingAsync(Booking booking);
        Task<bool> DeleteBookingAsync(int id);
        Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId);
        Task<IEnumerable<Booking>> GetBookingsByRoomIdAsync(int roomId);
        Task<bool> CheckRoomAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut);
        Task<bool> CheckRoomAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut, int excludeBookingId);
        Task<decimal> CalculateTotalPriceAsync(int roomId, DateTime checkIn, DateTime checkOut, int numberOfGuests);
        Task<IEnumerable<Booking>> GetUpcomingBookingsAsync();
        Task<IEnumerable<Booking>> GetPastBookingsAsync();
    }
}
