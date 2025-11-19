namespace Bookify.Repository.IRepository
{
    public interface IFavoriteRepository
    {
        Task<Favorite?> IsFavorite(string UserId, int RoomId);
        Task AddASync(Favorite favorite);
        Task RemoveAsync(Favorite favorite);
        Task<List<Favorite>> GetAllForUserAsync(string userId);
    }
}
