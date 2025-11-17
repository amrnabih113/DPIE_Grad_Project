using System.ComponentModel.DataAnnotations;

namespace Bookify.Models
{
    public class Testimonial
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required, MaxLength(500)]
        public string Message { get; set; }

        [Required, MaxLength(100)]
        public string Country { get; set; }

        [Required]
        public int Rating { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}

