using GuhaStore.Application.Services;
using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using GuhaStore.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GuhaStore.Web.Controllers.Admin;

[SessionAuthorization(1, 2)] // Staff or Admin
public class InventoryController : Controller
{
    private readonly IInventoryService _inventoryService;
    private readonly IUnitOfWork _unitOfWork;

    public InventoryController(IInventoryService inventoryService, IUnitOfWork unitOfWork)
    {
        _inventoryService = inventoryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index(int status = -1)
    {
        IEnumerable<Inventory> inventories;

        if (status == -1)
        {
            inventories = await _inventoryService.GetAllInventoriesAsync();
        }
        else
        {
            inventories = await _inventoryService.GetInventoriesByStatusAsync(status);
        }

        ViewBag.SelectedStatus = status;
        return View(inventories.OrderByDescending(i => i.InventoryDate));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var inventory = await _inventoryService.GetInventoryByIdAsync(id.Value);
        if (inventory == null)
        {
            return NotFound();
        }

        // Load inventory details
        var details = await _unitOfWork.InventoryDetails.GetAllAsync();
        ViewBag.InventoryDetails = details.Where(d => d.InventoryCode == inventory.InventoryCode).ToList();

        return View(inventory);
    }

    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inventory inventory, List<InventoryDetail> details)
    {
        if (ModelState.IsValid)
        {
            var accountId = GetAccountId();
            if (accountId.HasValue)
            {
                inventory.AccountId = accountId.Value;
            }

            inventory.InventoryDetails = details;
            await _inventoryService.CreateInventoryAsync(inventory);
            TempData["SuccessMessage"] = "Tạo phiếu nhập kho thành công.";
            return RedirectToAction(nameof(Index));
        }

        await LoadDropdowns();
        return View(inventory);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var inventory = await _inventoryService.GetInventoryByIdAsync(id.Value);
        if (inventory == null)
        {
            return NotFound();
        }

        await LoadDropdowns();
        return View(inventory);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Inventory inventory)
    {
        if (id != inventory.InventoryId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _inventoryService.UpdateInventoryAsync(inventory);
            TempData["SuccessMessage"] = "Cập nhật phiếu nhập kho thành công.";
            return RedirectToAction(nameof(Index));
        }

        await LoadDropdowns();
        return View(inventory);
    }

    public async Task<IActionResult> LowStock()
    {
        var lowStockProducts = await _inventoryService.GetLowStockProductsAsync(10);
        return View(lowStockProducts);
    }

    private async Task LoadDropdowns()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        ViewBag.Products = new SelectList(products.Where(p => p.ProductStatus == 1), "ProductId", "ProductName");
    }

    private int? GetAccountId()
    {
        return HttpContext.Session.GetInt32("AccountId");
    }
}

