using Bookify.Data.Models;
using Bookify.Repository;
using Bookify.services.IServices;

namespace Bookify.services
{
    public class BookingService : IBookingService
    {
        private readonly IGenericRepository<Booking> _bookingRepository;
        private readonly IGenericRepository<Room> _roomRepository;

        public BookingService(
            IGenericRepository<Booking> bookingRepository,
            IGenericRepository<Room> roomRepository)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _bookingRepository.GetAllIncludingAsync(
                b => b.User,
                b => b.Room);
        }

        public async Task<Booking> GetByIdAsync(int id)
        {
            var bookings = await _bookingRepository.FindAsync(
                b => b.Id == id,
                includeProperties: "User,Room,Room.RoomType,Room.RoomImages");
            return bookings.FirstOrDefault();
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            var isAvailable = await CheckRoomAvailabilityAsync(
                booking.RoomId,
                booking.CheckIn,
                booking.CheckOut);

            if (!isAvailable)
                throw new InvalidOperationException("Room is not available for the selected dates.");

            booking.TotalPrice = await CalculateTotalPriceAsync(
                booking.RoomId,
                booking.CheckIn,
                booking.CheckOut,
                booking.NumberOfGuests);

            booking.CreatedAt = DateTime.Now;
            booking.PaymentStatus = "Pending";

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            return booking;
        }

        public async Task<Booking> UpdateBookingAsync(Booking booking)
        {
            _bookingRepository.Update(booking);
            await _bookingRepository.SaveChangesAsync();
            return booking;
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return false;

            _bookingRepository.Delete(booking);
            await _bookingRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId)
        {
            return await _bookingRepository.FindAsync(
                b => b.UserId == userId,
                includeProperties: "Room,Room.RoomType,Room.RoomImages");
        }

        public async Task<IEnumerable<Booking>> GetBookingsByRoomIdAsync(int roomId)
        {
            return await _bookingRepository.FindAsync(
                b => b.RoomId == roomId,
                includeProperties: "User");
        }

        public async Task<bool> CheckRoomAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut)
        {
            var existingBookings = await _bookingRepository.FindAsync(
                b => b.RoomId == roomId &&
                     b.CheckIn < checkOut &&
                     b.CheckOut > checkIn &&
                     (b.PaymentStatus == "Pending" || b.PaymentStatus == "Confirmed"));

            return !existingBookings.Any();
        }

        public async Task<bool> CheckRoomAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut, int excludeBookingId)
        {
            var existingBookings = await _bookingRepository.FindAsync(
                b => b.RoomId == roomId &&
                     b.Id != excludeBookingId &&
                     b.CheckIn < checkOut &&
                     b.CheckOut > checkIn &&
                     (b.PaymentStatus == "Pending" || b.PaymentStatus == "Confirmed"));

            return !existingBookings.Any();
        }

        public async Task<decimal> CalculateTotalPriceAsync(int roomId, DateTime checkIn, DateTime checkOut, int numberOfGuests)
        {
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
                throw new InvalidOperationException("Room not found.");

            var numberOfNights = (checkOut.Date - checkIn.Date).Days;
            if (numberOfNights <= 0)
                throw new InvalidOperationException("Check-out date must be after check-in date.");

            decimal pricePerNight = room.HasDiscount && room.PriceAfterDiscount.HasValue
                ? room.PriceAfterDiscount.Value
                : room.PriceForNight;

            decimal totalPrice = pricePerNight * numberOfNights;

            if (numberOfGuests > 2)
            {
                var additionalGuests = numberOfGuests - 2;
                totalPrice += additionalGuests * 50 * numberOfNights;
            }

            return totalPrice;
        }

        public async Task<IEnumerable<Booking>> GetUpcomingBookingsAsync()
        {
            return await _bookingRepository.FindAsync(
                b => b.CheckIn > DateTime.Now &&
                     b.PaymentStatus != "Cancelled",
                includeProperties: "User,Room");
        }

        public async Task<IEnumerable<Booking>> GetPastBookingsAsync()
        {
            return await _bookingRepository.FindAsync(
                b => b.CheckOut <= DateTime.Now,
                includeProperties: "User,Room");
        }
    }
}
