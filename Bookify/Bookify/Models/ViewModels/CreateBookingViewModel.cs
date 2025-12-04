using System.ComponentModel.DataAnnotations;

namespace Bookify.Models.ViewModels
{
    public class CreateBookingViewModel
    {
        public int RoomId { get; set; }
        
        // Room Info (للعرض فقط)
        public string RoomName { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public string RoomImage { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }

        // Guest Info
        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; } = string.Empty;

        // Booking Details
        [Required(ErrorMessage = "Check-in date is required")]
        [Display(Name = "Check-in Date")]
        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }

        [Required(ErrorMessage = "Check-out date is required")]
        [Display(Name = "Check-out Date")]
        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }

        [Required(ErrorMessage = "Number of guests is required")]
        [Display(Name = "Number of Guests")]
        [Range(1, 10, ErrorMessage = "Number of guests must be between 1 and 10")]
        public int NumberOfGuests { get; set; }

        [Display(Name = "Special Requests")]
        [MaxLength(500, ErrorMessage = "Special requests cannot exceed 500 characters")]
        public string? SpecialRequests { get; set; }

        // Calculated values (للعرض)
        public int NumberOfNights => (CheckOut - CheckIn).Days;
        public decimal Subtotal => PricePerNight * NumberOfNights;
        public decimal Tax => Subtotal * 0.10m;
        public decimal TotalPrice => Subtotal + Tax;
    }
}
