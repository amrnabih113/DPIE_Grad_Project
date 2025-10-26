using Bookify.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Bookify.Data.Models
{
    public class RoomType
    {
        [Key]   
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        // Navigation
        public ICollection<Room>? Rooms { get; set; }
    }
}
