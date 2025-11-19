using Bookify.Custom_Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Bookify.Models.ViewModels
{
    public class AddRoomViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Room type is required")]
        [Display(Name ="Rome Type")]
        public int RoomTypeId { get; set; }
        [Display(Name ="Max Guests")]
        [Required(ErrorMessage = "Max guests is required")]
        [Range(1, 20, ErrorMessage = "Max guests must be between 1 and 20")]
        public int MaxGuests { get; set; }

        [Required(ErrorMessage = "Price per night is required")]
        [Range(1, 10000, ErrorMessage = "Price must be positive and less than 10000")]
        public decimal PriceForNight { get; set; }

        [Required(ErrorMessage = "Area is required")]
        [Range(5, 1000, ErrorMessage = "Area must be reasonable")]
        public double Area { get; set; }
        [Range(1, 10, ErrorMessage = "Number of beds must be between 1 and 10")]
        public int NoOfBeds { get; set; }
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Required(ErrorMessage = "Please upload at least one image")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        public List<IFormFile> RoomImages { get; set; } = new List<IFormFile>();

        // List for checkboxes
        public List<SelectListItem> RoomTypes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Amenities { get; set; } = new List<SelectListItem>();
        public List<int> SelectedAmenityIds { get; set; } = new List<int>();


        [Display(Name = "Has Discount?")]
        public bool HasDiscount { get; set; } = false;

        [Range(0, 100, ErrorMessage = "Discount must be between 0% and 100%")]
        [Display(Name = "Discount Percentage")]
        public int? DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Room number is required")]
        [Range(1, 9999, ErrorMessage = "Room number must be between 1 and 9999")]
        [Display(Name = "Room Number")]
        public int RoomNumber { get; set; }
    }
}
