
using Humanizer;

namespace Bookify.Repository
{
    public class RoomRepository : GenericRepository<Room>, IRoomRepository
    {
        public RoomRepository(ApplicationDbContext applicationDbContext):base(applicationDbContext) {
        }
        public async Task<IEnumerable<Room>> GetAllWithReviewsAsync()
        {
            return await _dbset.Include(r => r.Reviews).Include(r=>r.RoomImages).ToListAsync();
        }

        public async Task<List<Room>> GetByAllWithAllFeaturesAsync()
        {
            return await _dbset.Include(r => r.RoomType).Include(r => r.RoomImages)
                .Include(r => r.RoomAmenities)
                .ThenInclude(ra => ra.Amenity).ToListAsync();
        }

        public async Task<Room?> GetByIdWithAllFeaturesAsync(int id)
        {
            return await _dbset.Include(r => r.RoomType).Include(r => r.RoomImages)
                .Include(r => r.RoomAmenities)
                .ThenInclude(ra => ra.Amenity).FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Room?> GetByIdWithReviewsAsync(int id)
        {
            return await _dbset.Include(r=>r.Reviews).Include(r=>r.RoomImages).FirstOrDefaultAsync(r=>r.Id == id);
        }

        public async Task<IEnumerable<Room>> SearchWithFiltersAsync(int minPrice, int maxPrice, int MaxGuests, IEnumerable<RoomType> RoomTypes, IEnumerable<Amenity> Amenities)
        {
            var query = _dbset.Include(r => r.RoomType).Include(r=>r.RoomImages)
                .Include(r => r.RoomAmenities)
                .ThenInclude(ra => ra.Amenity)
                .AsQueryable();

            query = query.Where(r=>r.PriceForNight >= (decimal)minPrice &&  r.PriceForNight <= (decimal)maxPrice);
            query = query.Where(r => r.MaxGuests >= MaxGuests);

            if (RoomTypes != null && RoomTypes.Any())
            {
                var typeIds = RoomTypes.Select(t => t.Id).ToList();
                query = query.Where(r => typeIds.Contains(r.RoomTypeId));
            }

            if (Amenities != null && Amenities.Any())
            {
                var amenityIds = Amenities.Select(a => a.Id).ToList();
                foreach (var amenityId in amenityIds)
                    query = query.Where(r => r.RoomAmenities.Any(ra => ra.AmenityId == amenityId));
            }

            return await query.ToListAsync();
        }
    }
}
