namespace GuhaStore.Core.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(int id);

    // Extended methods for admin functionality
    Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, string? searchTerm = null, string? sortBy = null, bool ascending = true);
    Task<int> GetTotalCountAsync(string? searchTerm = null);
    Task UpdateStatusAsync(int id, int status);
}

