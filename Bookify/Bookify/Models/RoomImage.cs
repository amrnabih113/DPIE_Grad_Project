
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.Data.Models
{
    public class RoomImage
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey("Room")]
        public int RoomId { get; set; }

        [Required, Url]
        public string ImageUrl { get; set; }

        public Room Room { get; set; }
    }
}
