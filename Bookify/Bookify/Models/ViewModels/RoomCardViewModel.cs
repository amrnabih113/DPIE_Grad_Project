using System.ComponentModel.DataAnnotations;

namespace Bookify.Models.ViewModels
{
    public class RoomCardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int Rating { get; set; }
        public bool IsFavorite { get; set; } = false;
        [Display(Name ="Reviews")]
        public int NumberOfReviews { get; set; }
        public string Description { get; set; } = default!;
        [Display(Name ="Max Guests")]
        public int NumberOfGusts { get; set; }
        public int Area { get; set; }
        [Display(Name ="Price Per Night")]
        public int PricePerNight { get; set; }
        public decimal FinalPrice {  get; set; }
        public int? DiscountPercentage { get; set; }
        public string ImageUrl { get; set; } = default!;

        public IEnumerable<Amenity> Amenities { get; set; } = Enumerable.Empty<Amenity>();
        
    }
}
