using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Controllers
{
        [Authorize(Roles = "Admin")] // تأكد أن الرول اسمها Admin في الداتا بيز
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
                // 1. جلب البيانات
                var allBookings = await _bookingService.GetAllAsync();
                var allRooms = await _roomService.GetAllForAdminAsync();
                var user = await _userManager.GetUserAsync(User);

                // 2. تجهيز الـ ViewModel
                var model = new AdminDashboardViewModel
                {
                    AdminName = user != null ? user.FullName : "Admin",

                    // الإحصائيات
                    TotalBookings = allBookings.Count(),
                    // افترضنا أن الحالة Confirmed أو الدفع تم (غير Pending/Cancelled)
                    ConfirmedBookings = allBookings.Count(b => b.PaymentStatus == "Confirmed"),
                    AvailableRooms = allRooms.Count(r => r.IsAvailable),
                    TotalRevenue = allBookings
                                    .Where(b => b.PaymentStatus == "Confirmed")
                                    .Sum(b => b.TotalPrice),

                    // الجداول
                    RecentBookings = allBookings.OrderByDescending(b => b.CreatedAt).Take(5),
                    AllBookings = allBookings.OrderByDescending(b => b.CreatedAt),
                    Rooms = allRooms
                };

                return View(model);
            }
        }
}
