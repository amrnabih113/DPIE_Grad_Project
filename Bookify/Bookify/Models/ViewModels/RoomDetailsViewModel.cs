namespace Bookify.Models.ViewModels
{
    public class RoomDetailsViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int NumberOfGusts { get; set; }
        public int Area { get; set; }
        public decimal PricePerNight { get; set; }
        public int Rating { get; set; }
        public bool IsFavorite { get; set; }
        public int NumberOfReviews { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal FinalPrice { get; set; }
        public IEnumerable<Amenity> Amenities { get; set; } = default!;
        public IEnumerable<RoomImage> RoomImages { get; set; } = default!;

        // Review-related properties
        public List<ReviewViewModel> Reviews { get; set; } = new();
        public bool CanAddReview { get; set; }
        public bool HasUserReviewed { get; set; }

        // Booking status properties
        public bool IsBooked { get; set; }
        public DateTime? BookingCheckIn { get; set; }
        public DateTime? BookingCheckOut { get; set; }
        public int? DaysUntilAvailable { get; set; }
    }
}
