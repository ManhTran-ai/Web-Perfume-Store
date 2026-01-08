using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using GuhaStore.Application.Services;
using GuhaStore.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuhaStore.Application.Tests;

public class ProductServiceTests : IDisposable
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        // Setup in-memory database with unique name per test
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        // Setup mocks
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        // Setup memory cache
        _cache = new MemoryCache(new MemoryCacheOptions());

        _productService = new ProductService(_unitOfWorkMock.Object, _context, _cache);
    }

    public void Dispose()
    {
        _context.Dispose();
        _cache.Dispose();
    }

    [Fact]
    public async Task GetActiveProductsAsync_ReturnsOnlyActiveProducts()
    {
        // Arrange - Clear cache first
        _cache.Remove("ActiveProducts");

        var products = new List<Product>
        {
            new Product { ProductId = 100, ProductName = "Active Product", ProductStatus = 1 },
            new Product { ProductId = 101, ProductName = "Inactive Product", ProductStatus = 0 },
            new Product { ProductId = 102, ProductName = "Another Active Product", ProductStatus = 1 }
        };

        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _productService.GetActiveProductsAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal(1, p.ProductStatus));
    }

    [Fact]
    public async Task SearchProductsAsync_WithEmptyQuery_ReturnsEmptyList()
    {
        // Act
        var result = await _productService.SearchProductsAsync("");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchProductsAsync_WithValidQuery_ReturnsMatchingProducts()
    {
        // Arrange - Use a fresh context to avoid caching issues
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var freshContext = new ApplicationDbContext(options);
        var freshService = new ProductService(_unitOfWorkMock.Object, freshContext, _cache);

        var products = new List<Product>
        {
            new Product { ProductId = 20, ProductName = "iPhone 15", ProductDescription = "Latest smartphone", ProductStatus = 1 },
            new Product { ProductId = 21, ProductName = "Samsung Galaxy", ProductDescription = "Android phone", ProductStatus = 1 },
            new Product { ProductId = 22, ProductName = "MacBook Pro", ProductDescription = "Laptop computer", ProductStatus = 1 }
        };

        await freshContext.Products.AddRangeAsync(products);
        await freshContext.SaveChangesAsync();

        // Act
        var result = await freshService.SearchProductsAsync("phone");

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.ProductName.Contains("iPhone"));
        Assert.Contains(result, p => p.ProductDescription.Contains("phone"));
    }

    [Fact]
    public async Task GetAverageRatingAsync_WithNoReviews_ReturnsZero()
    {
        // Arrange - ensure clean database for this test
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        // Act
        var result = await _productService.GetAverageRatingAsync(999); // Non-existent product

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetReviewCountAsync_WithApprovedReviews_ReturnsCorrectCount()
    {
        // Arrange
        var reviews = new List<Evaluate>
        {
            new Evaluate { EvaluateId = 1, ProductId = 1, EvaluateRate = 5, EvaluateStatus = 1 },
            new Evaluate { EvaluateId = 2, ProductId = 1, EvaluateRate = 4, EvaluateStatus = 1 },
            new Evaluate { EvaluateId = 3, ProductId = 1, EvaluateRate = 3, EvaluateStatus = 0 }, // Pending
            new Evaluate { EvaluateId = 4, ProductId = 2, EvaluateRate = 5, EvaluateStatus = 1 }  // Different product
        };

        await _context.Evaluates.AddRangeAsync(reviews);
        await _context.SaveChangesAsync();

        // Act
        var count = await _productService.GetReviewCountAsync(1);
        var average = await _productService.GetAverageRatingAsync(1);

        // Assert
        Assert.Equal(2, count);
        Assert.Equal(4.5, average);
    }
}
