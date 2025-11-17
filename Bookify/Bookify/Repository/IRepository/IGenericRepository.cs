using System.Linq.Expressions;

namespace Bookify.Repository.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        //CRUD
        //I/O operation will be async to wait it
        Task AddAsync(T obj);
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllIncludingAsync(params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, string includeProperties = "");
        Task<IEnumerable<T>> GetAllAsync();
        void Update(T obj);
        void Delete(T obj);
        Task SaveChangesAsync();
    }
}