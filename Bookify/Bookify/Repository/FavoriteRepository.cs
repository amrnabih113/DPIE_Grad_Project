
namespace Bookify.Repository
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoriteRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public async Task AddASync(Favorite favorite)
        {
            await _context.AddAsync(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Favorite>> GetAllForUserAsync(string userId)
        {
            return await  _context.Favorites.Where(f=>f.UserId == userId)
                .Include(f=>f.Room)
                .ThenInclude(r=>r.RoomImages)
                .ToListAsync();
        }

        public async Task<Favorite?> IsFavorite(string UserId, int RoomId)
        {
            return await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == UserId && f.RoomId == RoomId);
        }

        public async Task RemoveAsync(Favorite favorite)
        {
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
        }
    }
}
