using GuhaStore.Core.Entities;

namespace GuhaStore.Core.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetActiveProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId);
    Task<IEnumerable<Product>> SearchProductsAsync(string query);
    Task<IEnumerable<Product>> GetFeaturedProductsAsync();
    Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId);
    Task<IEnumerable<ProductVariant>> GetProductVariantsAsync(int productId);
    Task<ProductVariant?> GetProductVariantAsync(int productId, int capacityId);
}

