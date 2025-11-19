using Bookify.services.IServices;

namespace Bookify.services
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IGenericRepository<RoomType> _roomTypeRopsitory;

        public RoomTypeService(IGenericRepository<RoomType> roomTypeRopsitory)
        {
            _roomTypeRopsitory = roomTypeRopsitory;
        }

        public async Task<IEnumerable<RoomType>> GetAllAsync()
        {
            return await _roomTypeRopsitory.GetAllAsync();
        }

        public async Task<RoomType> GetByIdAsync(int id)
        {
            return await _roomTypeRopsitory.GetByIdAsync(id);
        }

        public async Task<RoomType> AddAsync(RoomType entity)
        {
            await _roomTypeRopsitory.AddAsync(entity);
            await _roomTypeRopsitory.SaveChangesAsync();
            return entity;
        }

        public async Task<RoomType> UpdateAsync(RoomType entity)
        {
            _roomTypeRopsitory.Update(entity);
            await _roomTypeRopsitory.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var am = await _roomTypeRopsitory.GetByIdAsync(id);
            if (am == null) return false;

            _roomTypeRopsitory.Delete(am);
            await _roomTypeRopsitory.SaveChangesAsync();
            return true;
        }

    }
}
