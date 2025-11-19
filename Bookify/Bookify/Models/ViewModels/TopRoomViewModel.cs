namespace Bookify.Models.ViewModels
{
    public class TopRoomViewModel
    {
        public int Id { get; set; }  
        public string? Name { get; set; }
        public int PricePerNight { get; set; }
        public double Rating { get; set; }
        public string FirstImageUrl { get; set; } = string.Empty;

    }
}
