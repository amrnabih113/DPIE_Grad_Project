
public class AmenityService : IAmenityService
{
    private readonly IGenericRepository<Amenity> _amenityRepository;
    private readonly IGenericRepository<RoomAmenity> _roomAmenityRepository;

    public AmenityService(
        IGenericRepository<Amenity> amenityRepository,
        IGenericRepository<RoomAmenity> roomAmenityRepository)
    {
        _amenityRepository = amenityRepository;
        _roomAmenityRepository = roomAmenityRepository;
    }

    public async Task<IEnumerable<Amenity>> GetAllAsync()
    {
        return await _amenityRepository.GetAllAsync();
    }

    public async Task<Amenity> GetByIdAsync(int id)
    {
        return await _amenityRepository.GetByIdAsync(id);
    }

    public async Task<Amenity> AddAsync(Amenity entity)
    {
        await _amenityRepository.AddAsync(entity);
        await _amenityRepository.SaveChangesAsync();
        return entity;
    }

    public async Task<Amenity> UpdateAsync(Amenity entity)
    {
        _amenityRepository.Update(entity);
        await _amenityRepository.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var am = await _amenityRepository.GetByIdAsync(id);
        if (am == null) return false;

        _amenityRepository.Delete(am);
        await _amenityRepository.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Amenity>> GetAllWithRoomsAsync()
    {
        
        return await _amenityRepository
            .GetAllIncludingAsync(a => a.RoomAmenities)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<Amenity>> GetAmenitiesForRoomAsync(int roomId)
    {
        
        var roomAmenities = await _roomAmenityRepository
            .FindAsync(ra => ra.RoomId == roomId, includeProperties: "Amenity");

        return roomAmenities.Select(ra => ra.Amenity).Distinct().ToList();
    }
}