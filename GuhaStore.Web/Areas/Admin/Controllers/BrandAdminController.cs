using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using GuhaStore.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace GuhaStore.Web.Areas.Admin.Controllers;

[Area("Admin")]
[SessionAuthorization(1, 2)] // Staff or Admin
public class BrandAdminController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public BrandAdminController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var brands = await _unitOfWork.Brands.GetAllAsync();
        return View(brands.OrderBy(b => b.BrandId));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var brand = await _unitOfWork.Brands.GetByIdAsync(id.Value);
        if (brand == null)
        {
            return NotFound();
        }

        return View(brand);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Brand brand)
    {
        if (ModelState.IsValid)
        {
            await _unitOfWork.Brands.AddAsync(brand);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Brand created successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View(brand);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var brand = await _unitOfWork.Brands.GetByIdAsync(id.Value);
        if (brand == null)
        {
            return NotFound();
        }

        return View(brand);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Brand brand)
    {
        if (id != brand.BrandId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _unitOfWork.Brands.Update(brand);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Brand updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View(brand);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var brand = await _unitOfWork.Brands.GetByIdAsync(id.Value);
        if (brand == null)
        {
            return NotFound();
        }

        return View(brand);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var brand = await _unitOfWork.Brands.GetByIdAsync(id);
        if (brand != null)
        {
            _unitOfWork.Brands.Remove(brand);
            await _unitOfWork.SaveChangesAsync();
        }

        TempData["SuccessMessage"] = "Brand deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
