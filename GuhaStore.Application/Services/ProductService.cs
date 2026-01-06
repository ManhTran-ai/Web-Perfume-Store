using Microsoft.EntityFrameworkCore;
using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using GuhaStore.Infrastructure.Data;

namespace GuhaStore.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public ProductService(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.ProductStatus == 1)
            .ToListAsync();
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
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.ProductStatus == 1)
            .OrderByDescending(p => p.QuantitySales)
            .Take(8)
            .ToListAsync();
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
}

