using Microsoft.EntityFrameworkCore;
using GuhaStore.Core.Interfaces;
using GuhaStore.Infrastructure.Data;

namespace GuhaStore.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity != null;
    }

    public virtual async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, string? searchTerm = null, string? sortBy = null, bool ascending = true)
    {
        var query = _dbSet.AsQueryable();

        // Apply search if provided (basic implementation - can be extended)
        if (!string.IsNullOrEmpty(searchTerm))
        {
            // This is a basic implementation. In real scenarios, you might want to use
            // dynamic LINQ or search on specific properties
            query = query.Where(e => EF.Property<string>(e, "Name") != null &&
                                    EF.Property<string>(e, "Name").Contains(searchTerm));
        }

        // Apply sorting if provided
        if (!string.IsNullOrEmpty(sortBy))
        {
            query = ascending ? query.OrderBy(e => EF.Property<object>(e, sortBy))
                            : query.OrderByDescending(e => EF.Property<object>(e, sortBy));
        }

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public virtual async Task<int> GetTotalCountAsync(string? searchTerm = null)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(e => EF.Property<string>(e, "Name") != null &&
                                    EF.Property<string>(e, "Name").Contains(searchTerm));
        }

        return await query.CountAsync();
    }

    public virtual async Task UpdateStatusAsync(int id, int status)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            // Set the Status property dynamically
            var statusProperty = typeof(T).GetProperty("Status") ??
                               typeof(T).GetProperty("ProductStatus") ??
                               typeof(T).GetProperty("ArticleStatus") ??
                               typeof(T).GetProperty("EvaluateStatus") ??
                               typeof(T).GetProperty("CommentStatus");

            if (statusProperty != null && statusProperty.CanWrite)
            {
                statusProperty.SetValue(entity, status);
                _dbSet.Update(entity);
            }
        }
    }
}

