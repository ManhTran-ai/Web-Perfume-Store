using GuhaStore.Core.Entities;

namespace GuhaStore.Core.Interfaces;

public interface IWishlistService
{
    Task<bool> AddToWishlistAsync(int accountId, int productId);
    Task<bool> RemoveFromWishlistAsync(int accountId, int productId);
    Task<IEnumerable<Wishlist>> GetUserWishlistAsync(int accountId);
    Task<bool> IsInWishlistAsync(int accountId, int productId);
    Task<int> GetWishlistCountAsync(int accountId);
}
