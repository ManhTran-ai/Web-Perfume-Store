using GuhaStore.Core.Entities;

namespace GuhaStore.Core.Interfaces;

public interface IInventoryService
{
    Task<IEnumerable<Inventory>> GetAllInventoriesAsync();
    Task<Inventory?> GetInventoryByIdAsync(int id);
    Task<IEnumerable<Inventory>> GetInventoriesByStatusAsync(int status);
    Task<Inventory> CreateInventoryAsync(Inventory inventory);
    Task UpdateInventoryAsync(Inventory inventory);
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10);
}

