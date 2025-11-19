
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.Data.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        [Required]
        public int RoomNumber { get; set; }

        [Required]
        public int RoomTypeId { get; set; }

        [Required]
        public bool IsAvailable { get; set; } = true;

        public int FloorNumber { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceForNight { get; set; }

        [Required]
        public int MaxGuests { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Area { get; set; }

        [Required]
        public int NoOfBeds { get; set; }

        public bool HasDiscount { get; set; } = false;

        [Range(0, 100)]
        public int? DiscountPercent { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PriceAfterDiscount { get; set; }

        // Navigation
        public RoomType RoomType { get; set; }

        public ICollection<RoomImage>? RoomImages { get; set; }
        public ICollection<RoomAmenity>? RoomAmenities { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
    }
}
