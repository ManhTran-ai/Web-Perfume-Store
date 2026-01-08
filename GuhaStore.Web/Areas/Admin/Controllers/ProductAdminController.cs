using GuhaStore.Application.Services;
using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using GuhaStore.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GuhaStore.Web.Areas.Admin.Controllers;

[Area("Admin")]
[SessionAuthorization(1, 2)] // Staff or Admin
public class ProductAdminController : Controller
{
    private readonly IProductService _productService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileUploadService _fileUploadService;

    public ProductAdminController(
        IProductService productService,
        IUnitOfWork unitOfWork,
        IFileUploadService fileUploadService)
    {
        _productService = productService;
        _unitOfWork = unitOfWork;
        _fileUploadService = fileUploadService;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchTerm = "")
    {
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.SearchTerm = searchTerm;

        IEnumerable<Product> products;

        if (!string.IsNullOrEmpty(searchTerm))
        {
            products = await _productService.SearchProductsAsync(searchTerm);
        }
        else
        {
            products = await _productService.GetActiveProductsAsync();
        }

        var totalCount = products.Count();
        products = products.OrderByDescending(p => p.ProductId)
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize);

        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        ViewBag.TotalCount = totalCount;

        return View(products);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _productService.GetProductByIdAsync(id.Value);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdownLists();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                var imagePath = await _fileUploadService.UploadProductImageAsync(stream, imageFile.FileName);
                product.ProductImage = imagePath;
            }

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product created successfully.";
            return RedirectToAction(nameof(Index));
        }

        await PopulateDropdownLists();
        return View(product);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _unitOfWork.Products.GetByIdAsync(id.Value);
        if (product == null)
        {
            return NotFound();
        }

        await PopulateDropdownLists();
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
    {
        if (id != product.ProductId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                var imagePath = await _fileUploadService.UploadProductImageAsync(stream, imageFile.FileName);
                product.ProductImage = imagePath;
            }

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        await PopulateDropdownLists();
        return View(product);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _unitOfWork.Products.GetByIdAsync(id.Value);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product != null)
        {
            product.ProductStatus = 0; // Soft delete
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();
        }

        TempData["SuccessMessage"] = "Product deactivated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // Review Management
    public async Task<IActionResult> Reviews(int page = 1, int pageSize = 10, int status = -1)
    {
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.Status = status;

        var reviews = await _unitOfWork.Evaluates.GetAllAsync();

        // Apply status filter
        if (status != -1)
        {
            reviews = reviews.Where(r => r.EvaluateStatus == status);
        }

        var totalCount = reviews.Count();
        reviews = reviews.OrderByDescending(r => r.EvaluateId)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize);

        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        ViewBag.TotalCount = totalCount;

        return View(reviews);
    }

    [HttpPost]
    public async Task<IActionResult> ApproveReview(int reviewId)
    {
        var review = await _unitOfWork.Evaluates.GetByIdAsync(reviewId);
        if (review != null)
        {
            review.EvaluateStatus = 1; // Approved
            _unitOfWork.Evaluates.Update(review);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Review approved successfully.";
        }

        return RedirectToAction(nameof(Reviews));
    }

    [HttpPost]
    public async Task<IActionResult> RejectReview(int reviewId)
    {
        var review = await _unitOfWork.Evaluates.GetByIdAsync(reviewId);
        if (review != null)
        {
            review.EvaluateStatus = 2; // Rejected
            _unitOfWork.Evaluates.Update(review);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Review rejected.";
        }

        return RedirectToAction(nameof(Reviews));
    }

    private async Task PopulateDropdownLists()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        var brands = await _unitOfWork.Brands.GetAllAsync();
        var capacities = await _unitOfWork.Capacities.GetAllAsync();

        ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
        ViewBag.Brands = new SelectList(brands, "BrandId", "BrandName");
        ViewBag.Capacities = new SelectList(capacities, "CapacityId", "CapacityName");
    }
}
