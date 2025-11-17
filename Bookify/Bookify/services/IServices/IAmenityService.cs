public interface IAmenityService : IGenericService<Amenity>
{
    Task<IEnumerable<Amenity>> GetAllWithRoomsAsync();
    Task<IEnumerable<Amenity>> GetAmenitiesForRoomAsync(int roomId);
}