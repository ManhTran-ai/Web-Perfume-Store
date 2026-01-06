using System.Text.Json;
using GuhaStore.Core.Interfaces;
using GuhaStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace GuhaStore.Application.Services;

public class CartService : ICartService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;
    private const string CartSessionKey = "Cart";

    public CartService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    private ISession Session => _httpContextAccessor.HttpContext?.Session 
        ?? throw new InvalidOperationException("Session is not available");

    public async Task AddToCartAsync(int productId, int capacityId, int quantity)
    {
        var cart = GetCartFromSession();
        var variantKey = $"{productId}_{capacityId}";

        if (cart.ContainsKey(variantKey))
        {
            cart[variantKey] = (int)cart[variantKey]! + quantity;
        }
        else
        {
            cart[variantKey] = quantity;
        }

        SaveCartToSession(cart);
        await Task.CompletedTask;
    }

    public async Task UpdateCartItemQuantityAsync(string variantKey, int quantity)
    {
        var cart = GetCartFromSession();
        if (cart.ContainsKey(variantKey))
        {
            if (quantity <= 0)
            {
                cart.Remove(variantKey);
            }
            else
            {
                cart[variantKey] = quantity;
            }
            SaveCartToSession(cart);
        }
        await Task.CompletedTask;
    }

    public async Task RemoveFromCartAsync(string variantKey)
    {
        var cart = GetCartFromSession();
        cart.Remove(variantKey);
        SaveCartToSession(cart);
        await Task.CompletedTask;
    }

    public async Task<Dictionary<string, object>> GetCartItemsAsync()
    {
        var cart = GetCartFromSession();
        var cartItems = new Dictionary<string, object>();

        foreach (var item in cart)
        {
            var parts = item.Key.Split('_');
            if (parts.Length == 2 && 
                int.TryParse(parts[0], out int productId) && 
                int.TryParse(parts[1], out int capacityId))
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .FirstOrDefaultAsync(p => p.ProductId == productId);

                var variant = await _context.ProductVariants
                    .Include(pv => pv.Capacity)
                    .FirstOrDefaultAsync(pv => pv.ProductId == productId && pv.CapacityId == capacityId);

                if (product != null && variant != null)
                {
                    var price = variant.VariantPrice ?? product.ProductPrice;
                    var salePrice = price * (100 - product.ProductSale) / 100;
                    var total = salePrice * (int)item.Value!;

                    cartItems[item.Key] = new
                    {
                        ProductId = productId,
                        ProductName = product.ProductName,
                        CapacityId = capacityId,
                        CapacityName = variant.Capacity?.CapacityName ?? "",
                        Quantity = (int)item.Value!,
                        Price = price,
                        SalePrice = salePrice,
                        Total = total,
                        ProductImage = product.ProductImage
                    };
                }
            }
        }

        return cartItems;
    }

    public async Task<decimal> GetCartTotalAsync()
    {
        var cartItems = await GetCartItemsAsync();
        decimal total = 0;

        foreach (var item in cartItems.Values)
        {
            var itemDict = JsonSerializer.Deserialize<Dictionary<string, object>>(
                JsonSerializer.Serialize(item));
            if (itemDict != null && itemDict.ContainsKey("Total"))
            {
                if (decimal.TryParse(itemDict["Total"].ToString(), out decimal itemTotal))
                {
                    total += itemTotal;
                }
            }
        }

        return total;
    }

    public async Task<int> GetCartItemCountAsync()
    {
        var cart = GetCartFromSession();
        int count = 0;
        foreach (var item in cart.Values)
        {
            count += (int)item!;
        }
        return await Task.FromResult(count);
    }

    public async Task ClearCartAsync()
    {
        Session.Remove(CartSessionKey);
        await Task.CompletedTask;
    }

    public async Task<bool> ValidateCartAsync()
    {
        var cart = GetCartFromSession();
        
        foreach (var item in cart)
        {
            var parts = item.Key.Split('_');
            if (parts.Length == 2 && 
                int.TryParse(parts[0], out int productId) && 
                int.TryParse(parts[1], out int capacityId))
            {
                var variant = await _context.ProductVariants
                    .FirstOrDefaultAsync(pv => pv.ProductId == productId && 
                        pv.CapacityId == capacityId && 
                        pv.VariantStatus);

                if (variant == null || variant.VariantQuantity < (int)item.Value!)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private Dictionary<string, object> GetCartFromSession()
    {
        byte[]? cartBytes = null;
        if (Session.TryGetValue(CartSessionKey, out cartBytes) && cartBytes != null)
        {
            var cartJson = System.Text.Encoding.UTF8.GetString(cartBytes);
            if (!string.IsNullOrEmpty(cartJson))
            {
                try
                {
                    return JsonSerializer.Deserialize<Dictionary<string, object>>(cartJson) 
                        ?? new Dictionary<string, object>();
                }
                catch
                {
                    return new Dictionary<string, object>();
                }
            }
        }
        return new Dictionary<string, object>();
    }

    private void SaveCartToSession(Dictionary<string, object> cart)
    {
        var cartJson = JsonSerializer.Serialize(cart);
        var cartBytes = System.Text.Encoding.UTF8.GetBytes(cartJson);
        Session.Set(CartSessionKey, cartBytes);
    }
}

