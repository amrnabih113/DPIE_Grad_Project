using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Models.ViewModels
{
    public class RoomIndexViewModel
    {
        public IEnumerable<RoomCardViewModel> Rooms { get; set; } = default!;
        //filters : 
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public int? NumberOfGeusts { get; set; }
        public string? SelectedOrder { get; set; }

        public List<int> SelectedRoomTypeIds { get; set; } = new();
        public IEnumerable<SelectListItem> RoomTypes { get; set; } = new List<SelectListItem>();
        public List<int> SelectedAmenityIds { get; set; } = new();
        public IEnumerable<SelectListItem> Amenities { get; set; } = new List<SelectListItem>();


        //pagination 
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

    }
}
