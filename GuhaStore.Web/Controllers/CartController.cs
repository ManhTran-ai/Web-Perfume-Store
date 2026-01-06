using Microsoft.AspNetCore.Mvc;
using GuhaStore.Core.Interfaces;

namespace GuhaStore.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IProductService _productService;

    public CartController(ICartService cartService, IProductService productService)
    {
        _cartService = cartService;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var cartItems = await _cartService.GetCartItemsAsync();
        var total = await _cartService.GetCartTotalAsync();
        var isValid = await _cartService.ValidateCartAsync();

        ViewBag.CartTotal = total;
        ViewBag.IsValid = isValid;

        return View(cartItems);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId, int capacityId, int quantity = 1)
    {
        if (quantity <= 0)
        {
            TempData["ErrorMessage"] = "Số lượng không hợp lệ.";
            return RedirectToAction("Details", "Products", new { id = productId });
        }

        await _cartService.AddToCartAsync(productId, capacityId, quantity);
        TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng.";

        return RedirectToAction("Details", "Products", new { id = productId });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(string variantKey, int quantity)
    {
        if (quantity <= 0)
        {
            TempData["ErrorMessage"] = "Số lượng không hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        await _cartService.UpdateCartItemQuantityAsync(variantKey, quantity);
        TempData["SuccessMessage"] = "Cập nhật số lượng thành công.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveItem(string variantKey)
    {
        await _cartService.RemoveFromCartAsync(variantKey);
        TempData["SuccessMessage"] = "Đã xóa sản phẩm khỏi giỏ hàng.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Clear()
    {
        await _cartService.ClearCartAsync();
        TempData["SuccessMessage"] = "Đã xóa tất cả sản phẩm khỏi giỏ hàng.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetCartCount()
    {
        var count = await _cartService.GetCartItemCountAsync();
        return Json(new { count });
    }
}

