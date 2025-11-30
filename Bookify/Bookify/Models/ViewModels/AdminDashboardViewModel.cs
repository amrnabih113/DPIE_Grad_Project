namespace Bookify.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        // إحصائيات
        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int AvailableRooms { get; set; }
        public decimal TotalRevenue { get; set; }

        // اسم الأدمن للعرض
        public string AdminName { get; set; }

        // القوائم
        public IEnumerable<Booking> RecentBookings { get; set; }
        public IEnumerable<Booking> AllBookings { get; set; }
        public IEnumerable<Room> Rooms { get; set; }
    }
}
