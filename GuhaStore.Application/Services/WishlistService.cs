using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;

namespace GuhaStore.Application.Services;

public class WishlistService : IWishlistService
{
    private readonly IUnitOfWork _unitOfWork;

    public WishlistService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> AddToWishlistAsync(int accountId, int productId)
    {
        // Check if already exists
        var existing = await _unitOfWork.Wishlists.GetAllAsync();
        var wishlistItem = existing.FirstOrDefault(w => w.AccountId == accountId && w.ProductId == productId);

        if (wishlistItem != null)
        {
            return false; // Already in wishlist
        }

        var wishlist = new Wishlist
        {
            AccountId = accountId,
            ProductId = productId,
            AddedAt = DateTime.Now
        };

        await _unitOfWork.Wishlists.AddAsync(wishlist);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveFromWishlistAsync(int accountId, int productId)
    {
        var wishlists = await _unitOfWork.Wishlists.GetAllAsync();
        var wishlistItem = wishlists.FirstOrDefault(w => w.AccountId == accountId && w.ProductId == productId);

        if (wishlistItem == null)
        {
            return false; // Not in wishlist
        }

        await _unitOfWork.Wishlists.DeleteAsync(wishlistItem);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<Wishlist>> GetUserWishlistAsync(int accountId)
    {
        var wishlists = await _unitOfWork.Wishlists.GetAllAsync();
        return wishlists.Where(w => w.AccountId == accountId)
                       .OrderByDescending(w => w.AddedAt);
    }

    public async Task<bool> IsInWishlistAsync(int accountId, int productId)
    {
        var wishlists = await _unitOfWork.Wishlists.GetAllAsync();
        return wishlists.Any(w => w.AccountId == accountId && w.ProductId == productId);
    }

    public async Task<int> GetWishlistCountAsync(int accountId)
    {
        var wishlists = await _unitOfWork.Wishlists.GetAllAsync();
        return wishlists.Count(w => w.AccountId == accountId);
    }
}
