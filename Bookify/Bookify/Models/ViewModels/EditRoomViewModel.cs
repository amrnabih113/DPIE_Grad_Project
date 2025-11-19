using System.ComponentModel.DataAnnotations;

namespace Bookify.Models.ViewModels
{
    public class EditRoomViewModel:AddRoomViewModel
    {
        [Required]
        public int Id { get; set; }
        public new List<IFormFile>? RoomImages { get; set; } = new List<IFormFile>();
        public ICollection<RoomImage> ExistingRoomImages { get; set; } = new List<RoomImage>();
        public List<int> KeepImageIds { get; set; } = new List<int>();
    }
}
