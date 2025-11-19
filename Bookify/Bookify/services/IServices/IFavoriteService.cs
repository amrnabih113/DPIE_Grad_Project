namespace Bookify.services.IServices
{
    public interface IFavoriteService
    {
        Task<bool> ToggleFavoriteAsync(string UserId, int RoomId);
        Task<bool> IsFavoriteAsync(string UserId, int RoomId);
        Task<List<Favorite>> GetFavoritesForUser(string UserId);
    }
}
