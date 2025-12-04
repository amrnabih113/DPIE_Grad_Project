using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Controllers
{
        [Authorize(Roles = "Admin")]
        public class AdminController : Controller
        {
            private readonly IBookingService _bookingService;
            private readonly IRoomSerivce _roomService;
            private readonly UserManager<ApplicationUser> _userManager;

            public AdminController(
                IBookingService bookingService,
                IRoomSerivce roomService,
                UserManager<ApplicationUser> userManager)
            {
                _bookingService = bookingService;
                _roomService = roomService;
                _userManager = userManager;
            }

            public async Task<IActionResult> Index()
            {
                var allBookings = await _bookingService.GetAllAsync();
                var allRooms = await _roomService.GetAllForAdminAsync();
                var user = await _userManager.GetUserAsync(User);

                var model = new AdminDashboardViewModel
                {
                    AdminName = user != null ? user.FullName : "Admin",
                    TotalBookings = allBookings.Count(),
                    ConfirmedBookings = allBookings.Count(b => b.PaymentStatus == "Confirmed"),
                    AvailableRooms = allRooms.Count(r => r.IsAvailable),
                    TotalRevenue = allBookings
                                    .Where(b => b.PaymentStatus == "Confirmed" || b.PaymentStatus == "Completed")
                                    .Sum(b => b.TotalPrice),
                    RecentBookings = allBookings.OrderByDescending(b => b.CreatedAt).Take(5),
                    AllBookings = allBookings.OrderByDescending(b => b.CreatedAt),
                    Rooms = allRooms
                };

                return View(model);
            }

            [HttpPost]
            public async Task<IActionResult> UpdateBookingStatus([FromBody] UpdateBookingStatusRequest request)
            {
                try
                {
                    if (request == null || request.BookingId <= 0)
                    {
                        return Json(new { success = false, message = "Invalid request" });
                    }

                    var validStatuses = new[] { "Pending", "Confirmed", "Cancelled", "Completed" };
                    if (!validStatuses.Contains(request.Status))
                    {
                        return Json(new { success = false, message = "Invalid status" });
                    }

                    var booking = await _bookingService.GetByIdAsync(request.BookingId);
                    if (booking == null)
                    {
                        return Json(new { success = false, message = "Booking not found" });
                    }

                    booking.PaymentStatus = request.Status;
                    await _bookingService.UpdateBookingAsync(booking);

                    return Json(new { success = true, message = "Status updated successfully" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        public class UpdateBookingStatusRequest
        {
            public int BookingId { get; set; }
            public string Status { get; set; } = string.Empty;
        }
}
