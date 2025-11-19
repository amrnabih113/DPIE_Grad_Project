namespace Bookify.Repository.IRepository
{
    public interface IRoomRepository:IGenericRepository<Room>
    {
        Task <Room?> GetByIdWithReviewsAsync(int id);
        Task <IEnumerable<Room>> GetAllWithReviewsAsync();
        Task<Room?> GetByIdWithAllFeaturesAsync(int id);
        Task<List<Room>> GetByAllWithAllFeaturesAsync();
        
        Task<IEnumerable<Room>> SearchWithFiltersAsync(int minPrice,int maxPrice,int MaxGuests, IEnumerable<RoomType> RoomTypes, IEnumerable<Amenity> aAmenities);
    }
}
