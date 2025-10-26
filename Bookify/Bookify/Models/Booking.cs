
using Bookify.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.Data.Models
{
   
        public class Booking
        {
            [Key]
            public int Id { get; set; }

            [Required, ForeignKey("User")]
            public string UserId { get; set; }

            [Required, ForeignKey("Room")]
            public int RoomId { get; set; }

            [Required, DataType(DataType.DateTime)]
            public DateTime CheckIn { get; set; }

            [Required, DataType(DataType.DateTime)]
            public DateTime CheckOut { get; set; }

            [Column(TypeName = "decimal(18,2)")]
            public decimal TotalPrice { get; set; }

            [MaxLength(50)]
            public string? PaymentStatus { get; set; }

            [MaxLength(50)]
            public string? PaymentMethod { get; set; }

            public DateTime? PaymentDate { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.Now;

            // Navigation
            public ApplicationUser User { get; set; }
            public Room Room { get; set; }
        }
    }

