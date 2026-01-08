using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using GuhaStore.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace GuhaStore.Web.Areas.Admin.Controllers;

[Area("Admin")]
[SessionAuthorization(1, 2)] // Staff or Admin
public class CategoryAdminController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileUploadService _fileUploadService;

    public CategoryAdminController(IUnitOfWork unitOfWork, IFileUploadService fileUploadService)
    {
        _unitOfWork = unitOfWork;
        _fileUploadService = fileUploadService;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return View(categories.OrderBy(c => c.CategoryId));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _unitOfWork.Categories.GetByIdAsync(id.Value);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                var imagePath = await _fileUploadService.UploadCategoryImageAsync(stream, imageFile.FileName);
                category.CategoryImage = imagePath;
            }

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category created successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View(category);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _unitOfWork.Categories.GetByIdAsync(id.Value);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category, IFormFile? imageFile)
    {
        if (id != category.CategoryId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                var imagePath = await _fileUploadService.UploadCategoryImageAsync(stream, imageFile.FileName);
                category.CategoryImage = imagePath;
            }

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View(category);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _unitOfWork.Categories.GetByIdAsync(id.Value);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category != null)
        {
            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.SaveChangesAsync();
        }

        TempData["SuccessMessage"] = "Category deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
