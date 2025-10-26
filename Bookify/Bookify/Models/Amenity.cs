using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bookify.Models
{
    public class Amenity
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        // Navigation
        public ICollection<RoomAmenity>? RoomAmenities { get; set; }
    }
}

