using Bookify.Data.Models;
using Bookify.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RoomAmenity
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Room")]
        public int RoomId { get; set; }

        [Required, ForeignKey("Amenity")]
        public int AmenityId { get; set; }

        public Room Room { get; set; }
        public Amenity Amenity { get; set; }
    }

