using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using GuhaStore.Infrastructure.Data;

namespace GuhaStore.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private const string ActiveProductsCacheKey = "ActiveProducts";
    private const string FeaturedProductsCacheKey = "FeaturedProducts";
    private readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public ProductService(IUnitOfWork unitOfWork, ApplicationDbContext context, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _cache = cache;
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        if (!_cache.TryGetValue(ActiveProductsCacheKey, out IEnumerable<Product>? products))
        {
            products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.ProductStatus == 1)
                .ToListAsync();

            _cache.Set(ActiveProductsCacheKey, products, CacheDuration);
        }

        return products ?? new List<Product>();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.ProductVariants)
                .ThenInclude(pv => pv.Capacity)
            .Include(p => p.Evaluates.Where(e => e.EvaluateStatus == 1))
            .FirstOrDefaultAsync(p => p.ProductId == id);
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.ProductCategory == categoryId && p.ProductStatus == 1)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.ProductBrand == brandId && p.ProductStatus == 1)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<Product>();

        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.ProductStatus == 1 && 
                (p.ProductName.Contains(query) || 
                 p.ProductDescription.Contains(query)))
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
    {
        if (!_cache.TryGetValue(FeaturedProductsCacheKey, out IEnumerable<Product>? products))
        {
            products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.ProductStatus == 1)
                .OrderByDescending(p => p.QuantitySales)
                .Take(8)
                .ToListAsync();

            _cache.Set(FeaturedProductsCacheKey, products, CacheDuration);
        }

        return products ?? new List<Product>();
    }

    public async Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            return new List<Product>();

        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.ProductId != productId && 
                p.ProductStatus == 1 &&
                (p.ProductCategory == product.ProductCategory || 
                 p.ProductBrand == product.ProductBrand))
            .Take(4)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductVariant>> GetProductVariantsAsync(int productId)
    {
        return await _context.ProductVariants
            .Include(pv => pv.Capacity)
            .Where(pv => pv.ProductId == productId && pv.VariantStatus)
            .ToListAsync();
    }

    public async Task<ProductVariant?> GetProductVariantAsync(int productId, int capacityId)
    {
        return await _context.ProductVariants
            .Include(pv => pv.Capacity)
            .FirstOrDefaultAsync(pv => pv.ProductId == productId &&
                pv.CapacityId == capacityId &&
                pv.VariantStatus);
    }

    public async Task<bool> AddProductReviewAsync(int accountId, int productId, int rating, string content)
    {
        // Check if user already reviewed this product
        var existingReview = await _context.Evaluates
            .FirstOrDefaultAsync(e => e.AccountId == accountId && e.ProductId == productId);

        if (existingReview != null)
        {
            // Update existing review
            existingReview.EvaluateRate = rating;
            existingReview.EvaluateContent = content;
            existingReview.EvaluateDate = DateTime.Now.ToString("yyyy-MM-dd");
            existingReview.EvaluateStatus = 0; // Pending approval

            _context.Evaluates.Update(existingReview);
        }
        else
        {
            // Create new review
            var review = new Evaluate
            {
                AccountId = accountId,
                ProductId = productId,
                AccountName = "", // Will be set from account
                EvaluateRate = rating,
                EvaluateContent = content,
                EvaluateDate = DateTime.Now.ToString("yyyy-MM-dd"),
                EvaluateStatus = 0 // Pending approval
            };

            await _context.Evaluates.AddAsync(review);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Evaluate>> GetProductReviewsAsync(int productId, bool approvedOnly = true)
    {
        var query = _context.Evaluates
            .Include(e => e.Account)
            .Where(e => e.ProductId == productId);

        if (approvedOnly)
        {
            query = query.Where(e => e.EvaluateStatus == 1);
        }

        return await query.OrderByDescending(e => e.EvaluateDate).ToListAsync();
    }

    public async Task<double> GetAverageRatingAsync(int productId)
    {
        var reviews = await _context.Evaluates
            .Where(e => e.ProductId == productId && e.EvaluateStatus == 1)
            .ToListAsync();

        if (!reviews.Any())
            return 0;

        return reviews.Average(r => r.EvaluateRate);
    }

    public async Task<int> GetReviewCountAsync(int productId)
    {
        return await _context.Evaluates
            .CountAsync(e => e.ProductId == productId && e.EvaluateStatus == 1);
    }
}

