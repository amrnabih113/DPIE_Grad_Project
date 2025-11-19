namespace Bookify.services.IServices
{
    public interface IRoomSerivce:IGenericService<Room>
    {
        Task<IEnumerable<RoomCardViewModel>> AllForCards();
        Task<IEnumerable<RoomCardViewModel>> SerachWithFilterAsync(int minPrice,int maxPrice,int maxGuests,IEnumerable<RoomType> roomTypes,IEnumerable<Amenity> amenities);
        Task<RoomDetailsViewModel> GetRoomDetailsAsync(int id);
        Task<IEnumerable<TopRoomViewModel>> GetTopRatedRoomsAsync(int top = 3);
        Task AddRoomAsync(AddRoomViewModel model);
        Task<EditRoomViewModel> GetRoomForEditAsync(int id);
        public Task UpdateRoomAsync(EditRoomViewModel model);


    }
}
