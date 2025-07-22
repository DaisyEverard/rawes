using backend.src.Data;

namespace backend.src.Repositories
{
    public interface IBigQueryRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(string id);
        Task InsertAsync(T entity);
    }
}
