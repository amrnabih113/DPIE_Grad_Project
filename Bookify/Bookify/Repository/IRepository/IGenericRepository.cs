namespace Bookify.Repository.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        //CRUD
        //I/O operation will be async to wait it
        Task AddAsync(T obj);
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        void Update(T obj);
        void Delete(T obj);
        Task SaveChangesAsync();
    }
}
