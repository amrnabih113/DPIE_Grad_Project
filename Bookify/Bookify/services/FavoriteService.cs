using Bookify.services.IServices;

namespace Bookify.services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRop;

        public FavoriteService(IFavoriteRepository favorite)
        {
            _favoriteRop = favorite;
        }
        public async Task<List<Favorite>> GetFavoritesForUser(string UserId)
        {
            return await _favoriteRop.GetAllForUserAsync(UserId);
        }

        public async Task<bool> IsFavoriteAsync(string UserId, int RoomId)
        {
            return await _favoriteRop.IsFavorite(UserId, RoomId) != null;
        }

        public async Task<bool> ToggleFavoriteAsync(string UserId, int RoomId)
        {
            var favorite = await _favoriteRop.IsFavorite(UserId, RoomId);
            if (favorite != null)
            {
                await _favoriteRop.RemoveAsync(favorite);
                return false; // removed
            }
            else
            {
                await _favoriteRop.AddASync(new Favorite { UserId = UserId, RoomId = RoomId});
                return true; // added
            }
        }
    }
}
