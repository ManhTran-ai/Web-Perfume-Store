using GuhaStore.Core.Entities;

namespace GuhaStore.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Account> Accounts { get; }
    IRepository<Product> Products { get; }
    IRepository<ProductVariant> ProductVariants { get; }
    IRepository<Category> Categories { get; }
    IRepository<Brand> Brands { get; }
    IRepository<Capacity> Capacities { get; }
    IRepository<Collection> Collections { get; }
    IRepository<Order> Orders { get; }
    IRepository<OrderDetail> OrderDetails { get; }
    IRepository<Customer> Customers { get; }
    IRepository<Delivery> Deliveries { get; }
    IRepository<Article> Articles { get; }
    IRepository<Comment> Comments { get; }
    IRepository<Evaluate> Evaluates { get; }
    IRepository<Inventory> Inventories { get; }
    IRepository<InventoryDetail> InventoryDetails { get; }
    IRepository<Metric> Metrics { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

