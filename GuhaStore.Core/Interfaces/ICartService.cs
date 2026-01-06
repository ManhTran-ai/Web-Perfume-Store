namespace GuhaStore.Core.Interfaces;

public interface ICartService
{
    Task AddToCartAsync(int productId, int capacityId, int quantity);
    Task UpdateCartItemQuantityAsync(string variantKey, int quantity);
    Task RemoveFromCartAsync(string variantKey);
    Task<Dictionary<string, object>> GetCartItemsAsync();
    Task<decimal> GetCartTotalAsync();
    Task<int> GetCartItemCountAsync();
    Task ClearCartAsync();
    Task<bool> ValidateCartAsync();
}

