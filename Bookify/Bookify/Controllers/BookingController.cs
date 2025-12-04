using Bookify.Data.Models;
using Bookify.Models.ViewModels;
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
        private readonly IRoomSerivce _roomService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<BookingController> _logger;

        public BookingController(
            IBookingService bookingService,
            IRoomSerivce roomService,
            UserManager<ApplicationUser> userManager,
            ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _roomService = roomService;
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult MyBookings(string status = "all")
        {
            return RedirectToAction(nameof(Index), new { status });
        }

        public async Task<IActionResult> Index(string status = "all")
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                var bookings = await _bookingService.GetBookingsByUserIdAsync(user.Id);
                
                // Apply status filter
                if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
                {
                    bookings = bookings.Where(b => 
                        string.Equals(b.PaymentStatus, status, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                
                ViewBag.StatusFilter = status;
                return View(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings for user");
                return BadRequest("An error occurred while retrieving bookings.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var booking = await _bookingService.GetByIdAsync(id);
                if (booking == null)
                {
                    TempData["ErrorMessage"] = "Booking not found.";
                    return RedirectToAction(nameof(Index));
                }

                var user = await _userManager.GetUserAsync(User);
                if (booking.UserId != user.Id && !User.IsInRole("Admin"))
                {
                    TempData["ErrorMessage"] = "You are not authorized to cancel this booking.";
                    return RedirectToAction(nameof(Index));
                }

                if (booking.CheckIn <= DateTime.Now)
                {
                    TempData["ErrorMessage"] = "Cannot cancel a booking that has already started or is in the past.";
                    return RedirectToAction(nameof(Index));
                }

                if (booking.PaymentStatus?.ToLower() == "cancelled")
                {
                    TempData["ErrorMessage"] = "This booking is already cancelled.";
                    return RedirectToAction(nameof(Index));
                }

                booking.PaymentStatus = "Cancelled";
                await _bookingService.UpdateBookingAsync(booking);
                
                TempData["SuccessMessage"] = "Your booking has been cancelled successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking");
                TempData["ErrorMessage"] = "An error occurred while cancelling the booking.";
                return RedirectToAction(nameof(Index));
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

        // GET: Booking/Create?roomId=1
        public async Task<IActionResult> Create(int? roomId)
        {
            try
            {
                if (!roomId.HasValue)
                {
                    return RedirectToAction("Index", "Room");
                }

                var room = await _roomService.GetByIdAsync(roomId.Value);
                if (room == null)
                {
                    return NotFound("Room not found");
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Create", "Booking", new { roomId }) });
                }

                // Get room image
                var roomImage = room.RoomImages?.FirstOrDefault()?.ImageUrl ?? "/images/placeholder-room.jpg";

                var viewModel = new CreateBookingViewModel
                {
                    RoomId = room.Id,
                    RoomName = room.Name ?? "Room",
                    RoomType = room.RoomType?.Name ?? "Standard",
                    RoomImage = roomImage,
                    PricePerNight = room.PriceForNight,
                    MaxGuests = room.MaxGuests,
                    FirstName = user.FullName?.Split(' ').FirstOrDefault() ?? "",
                    LastName = user.FullName?.Split(' ').Skip(1).FirstOrDefault() ?? "",
                    Email = user.Email ?? "",
                    Phone = user.PhoneNumber ?? "",
                    CheckIn = DateTime.Today,
                    CheckOut = DateTime.Today.AddDays(1)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading booking form");
                return BadRequest("An error occurred while loading the booking form.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookingViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                // Validate dates
                if (model.CheckOut <= model.CheckIn)
                {
                    ModelState.AddModelError("CheckOut", "Check-out date must be after check-in date");
                }

                if (model.CheckIn < DateTime.Today)
                {
                    ModelState.AddModelError("CheckIn", "Check-in date cannot be in the past");
                }

                if (ModelState.IsValid)
                {
                    // Calculate total price
                    var nights = (model.CheckOut - model.CheckIn).Days;
                    var subtotal = model.PricePerNight * nights;
                    var tax = subtotal * 0.10m;
                    var totalPrice = subtotal + tax;

                    var booking = new Booking
                    {
                        RoomId = model.RoomId,
                        UserId = user.Id,
                        CheckIn = model.CheckIn,
                        CheckOut = model.CheckOut,
                        NumberOfGuests = model.NumberOfGuests,
                        TotalPrice = totalPrice,
                        PaymentStatus = "Pending",
                        CreatedAt = DateTime.Now
                    };

                    var createdBooking = await _bookingService.CreateBookingAsync(booking);
                    
                    TempData["SuccessMessage"] = "Booking confirmed successfully!";
                    return RedirectToAction(nameof(Details), new { id = createdBooking.Id });
                }

                // If validation fails, reload room data
                var room = await _roomService.GetByIdAsync(model.RoomId);
                if (room != null)
                {
                    model.RoomName = room.Name ?? "Room";
                    model.RoomType = room.RoomType?.Name ?? "Standard";
                    model.RoomImage = room.RoomImages?.FirstOrDefault()?.ImageUrl ?? "/images/placeholder-room.jpg";
                    model.MaxGuests = room.MaxGuests;
                }

                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                ModelState.AddModelError("", "An error occurred while creating the booking.");
                return View(model);
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
