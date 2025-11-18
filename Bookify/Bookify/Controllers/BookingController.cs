using Bookify.Data.Models;
using Bookify.services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<BookingController> _logger;

        public BookingController(
            IBookingService bookingService,
            UserManager<ApplicationUser> userManager,
            ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                var bookings = await _bookingService.GetBookingsByUserIdAsync(user.Id);
                return View(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings for user");
                return BadRequest("An error occurred while retrieving bookings.");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var booking = await _bookingService.GetByIdAsync(id);
                if (booking == null)
                    return NotFound();

                var user = await _userManager.GetUserAsync(User);
                if (booking.UserId != user.Id && !User.IsInRole("Admin"))
                    return Forbid();

                return View(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking details");
                return BadRequest("An error occurred while retrieving booking details.");
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomId,CheckIn,CheckOut,NumberOfGuests")] Booking booking)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                if (ModelState.IsValid)
                {
                    booking.UserId = user.Id;
                    var createdBooking = await _bookingService.CreateBookingAsync(booking);
                    return RedirectToAction(nameof(Details), new { id = createdBooking.Id });
                }

                return View(booking);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                ModelState.AddModelError("", "An error occurred while creating the booking.");
                return View(booking);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var booking = await _bookingService.GetByIdAsync(id);
                if (booking == null)
                    return NotFound();

                var user = await _userManager.GetUserAsync(User);
                if (booking.UserId != user.Id && !User.IsInRole("Admin"))
                    return Forbid();

                return View(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking for edit");
                return BadRequest("An error occurred while retrieving the booking.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,RoomId,CheckIn,CheckOut,NumberOfGuests,TotalPrice,PaymentStatus,PaymentMethod,PaymentDate,CreatedAt")] Booking booking)
        {
            if (id != booking.Id)
                return NotFound();

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var existingBooking = await _bookingService.GetByIdAsync(id);

                if (existingBooking.UserId != user.Id && !User.IsInRole("Admin"))
                    return Forbid();

                if (ModelState.IsValid)
                {
                    await _bookingService.UpdateBookingAsync(booking);
                    return RedirectToAction(nameof(Details), new { id = booking.Id });
                }

                return View(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking");
                ModelState.AddModelError("", "An error occurred while updating the booking.");
                return View(booking);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var booking = await _bookingService.GetByIdAsync(id);
                if (booking == null)
                    return NotFound();

                var user = await _userManager.GetUserAsync(User);
                if (booking.UserId != user.Id && !User.IsInRole("Admin"))
                    return Forbid();

                return View(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking for deletion");
                return BadRequest("An error occurred while retrieving the booking.");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var booking = await _bookingService.GetByIdAsync(id);
                if (booking == null)
                    return NotFound();

                var user = await _userManager.GetUserAsync(User);
                if (booking.UserId != user.Id && !User.IsInRole("Admin"))
                    return Forbid();

                await _bookingService.DeleteBookingAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting booking");
                return BadRequest("An error occurred while deleting the booking.");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> CheckAvailability(int roomId, DateTime checkIn, DateTime checkOut)
        {
            try
            {
                var isAvailable = await _bookingService.CheckRoomAvailabilityAsync(roomId, checkIn, checkOut);
                return Json(new { available = isAvailable });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking room availability");
                return Json(new { available = false, error = "An error occurred while checking availability." });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> CalculatePrice(int roomId, DateTime checkIn, DateTime checkOut, int numberOfGuests)
        {
            try
            {
                var totalPrice = await _bookingService.CalculateTotalPriceAsync(roomId, checkIn, checkOut, numberOfGuests);
                return Json(new { totalPrice = totalPrice });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating booking price");
                return Json(new { error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpcomingBookings()
        {
            try
            {
                var bookings = await _bookingService.GetUpcomingBookingsAsync();
                return View(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving upcoming bookings");
                return BadRequest("An error occurred while retrieving upcoming bookings.");
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PastBookings()
        {
            try
            {
                var bookings = await _bookingService.GetPastBookingsAsync();
                return View(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving past bookings");
                return BadRequest("An error occurred while retrieving past bookings.");
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RoomBookings(int roomId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByRoomIdAsync(roomId);
                return View(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving room bookings");
                return BadRequest("An error occurred while retrieving room bookings.");
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllBookings()
        {
            try
            {
                var bookings = await _bookingService.GetAllAsync();
                return View(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all bookings");
                return BadRequest("An error occurred while retrieving all bookings.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int id)
        {
            try
            {
                var booking = await _bookingService.GetByIdAsync(id);
                if (booking == null)
                    return NotFound();

                var user = await _userManager.GetUserAsync(User);
                if (booking.UserId != user.Id && !User.IsInRole("Admin"))
                    return Forbid();

                booking.PaymentStatus = "Cancelled";
                await _bookingService.UpdateBookingAsync(booking);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking");
                return BadRequest("An error occurred while cancelling the booking.");
            }
        }
    }
}
