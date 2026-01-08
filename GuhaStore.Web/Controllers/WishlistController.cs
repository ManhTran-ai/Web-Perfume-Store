using GuhaStore.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GuhaStore.Web.Controllers;

public class WishlistController : Controller
{
    private readonly IWishlistService _wishlistService;
    private readonly IProductService _productService;

    public WishlistController(IWishlistService wishlistService, IProductService productService)
    {
        _wishlistService = wishlistService;
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (accountId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var wishlist = await _wishlistService.GetUserWishlistAsync(accountId.Value);
        return View(wishlist);
    }

    [HttpPost]
    public async Task<IActionResult> Toggle(int productId)
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (accountId == null)
        {
            return Json(new { success = false, message = "Please login first" });
        }

        var isInWishlist = await _wishlistService.IsInWishlistAsync(accountId.Value, productId);

        if (isInWishlist)
        {
            // Remove from wishlist
            var removed = await _wishlistService.RemoveFromWishlistAsync(accountId.Value, productId);
            return Json(new { success = removed, action = "removed" });
        }
        else
        {
            // Add to wishlist
            var added = await _wishlistService.AddToWishlistAsync(accountId.Value, productId);
            return Json(new { success = added, action = "added" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int productId)
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (accountId == null)
        {
            return Json(new { success = false, message = "Please login first" });
        }

        var removed = await _wishlistService.RemoveFromWishlistAsync(accountId.Value, productId);
        return Json(new { success = removed });
    }

    [HttpGet]
    public async Task<IActionResult> IsInWishlist(int productId)
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (accountId == null)
        {
            return Json(new { isInWishlist = false });
        }

        var isInWishlist = await _wishlistService.IsInWishlistAsync(accountId.Value, productId);
        return Json(new { isInWishlist });
    }

    [HttpGet]
    public async Task<IActionResult> Count()
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (accountId == null)
        {
            return Json(new { count = 0 });
        }

        var count = await _wishlistService.GetWishlistCountAsync(accountId.Value);
        return Json(new { count });
    }
}
