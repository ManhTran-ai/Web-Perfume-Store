using Microsoft.EntityFrameworkCore.Storage;
using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using GuhaStore.Infrastructure.Data;
using GuhaStore.Infrastructure.Repositories;

namespace GuhaStore.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private IRepository<Account>? _accounts;
    private IRepository<Product>? _products;
    private IRepository<ProductVariant>? _productVariants;
    private IRepository<Category>? _categories;
    private IRepository<Brand>? _brands;
    private IRepository<Capacity>? _capacities;
    private IRepository<Collection>? _collections;
    private IRepository<Order>? _orders;
    private IRepository<OrderDetail>? _orderDetails;
    private IRepository<Customer>? _customers;
    private IRepository<Delivery>? _deliveries;
    private IRepository<Article>? _articles;
    private IRepository<Comment>? _comments;
    private IRepository<Evaluate>? _evaluates;
    private IRepository<Inventory>? _inventories;
    private IRepository<InventoryDetail>? _inventoryDetails;
    private IRepository<Metric>? _metrics;
    private IRepository<PaymentTransaction>? _paymentTransactions;
    private IRepository<Wishlist>? _wishlists;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<Account> Accounts => _accounts ??= new Repository<Account>(_context);
    public IRepository<Product> Products => _products ??= new Repository<Product>(_context);
    public IRepository<ProductVariant> ProductVariants => _productVariants ??= new Repository<ProductVariant>(_context);
    public IRepository<Category> Categories => _categories ??= new Repository<Category>(_context);
    public IRepository<Brand> Brands => _brands ??= new Repository<Brand>(_context);
    public IRepository<Capacity> Capacities => _capacities ??= new Repository<Capacity>(_context);
    public IRepository<Collection> Collections => _collections ??= new Repository<Collection>(_context);
    public IRepository<Order> Orders => _orders ??= new Repository<Order>(_context);
    public IRepository<OrderDetail> OrderDetails => _orderDetails ??= new Repository<OrderDetail>(_context);
    public IRepository<Customer> Customers => _customers ??= new Repository<Customer>(_context);
    public IRepository<Delivery> Deliveries => _deliveries ??= new Repository<Delivery>(_context);
    public IRepository<Article> Articles => _articles ??= new Repository<Article>(_context);
    public IRepository<Comment> Comments => _comments ??= new Repository<Comment>(_context);
    public IRepository<Evaluate> Evaluates => _evaluates ??= new Repository<Evaluate>(_context);
    public IRepository<Inventory> Inventories => _inventories ??= new Repository<Inventory>(_context);
    public IRepository<InventoryDetail> InventoryDetails => _inventoryDetails ??= new Repository<InventoryDetail>(_context);
    public IRepository<Metric> Metrics => _metrics ??= new Repository<Metric>(_context);
    public IRepository<PaymentTransaction> PaymentTransactions => _paymentTransactions ??= new Repository<PaymentTransaction>(_context);
    public IRepository<Wishlist> Wishlists => _wishlists ??= new Repository<Wishlist>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

