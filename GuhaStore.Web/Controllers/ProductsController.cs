using GuhaStore.Application.Services;
using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GuhaStore.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly IUnitOfWork _unitOfWork;

    public ProductsController(IProductService productService, IUnitOfWork unitOfWork)
    {
        _productService = productService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index(int? categoryId, int? brandId, string? searchTerm, int page = 1, int pageSize = 12, string sortBy = "newest", int? minPrice = null, int? maxPrice = null, bool onSaleOnly = false)
    {
        ViewBag.CategoryId = categoryId;
        ViewBag.BrandId = brandId;
        ViewBag.SearchTerm = searchTerm;
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.SortBy = sortBy;
        ViewBag.MinPrice = minPrice;
        ViewBag.MaxPrice = maxPrice;
        ViewBag.OnSaleOnly = onSaleOnly;

        IEnumerable<Product> products;

        if (!string.IsNullOrEmpty(searchTerm))
        {
            products = await _productService.SearchProductsAsync(searchTerm);
        }
        else if (categoryId.HasValue)
        {
            products = await _productService.GetProductsByCategoryAsync(categoryId.Value);
        }
        else if (brandId.HasValue)
        {
            products = await _productService.GetProductsByBrandAsync(brandId.Value);
        }
        else
        {
            products = await _productService.GetActiveProductsAsync();
        }

        if (minPrice.HasValue)
            products = products.Where(p => (p.ProductPrice - (p.ProductPrice * p.ProductSale / 100)) >= minPrice.Value);
        if (maxPrice.HasValue)
            products = products.Where(p => (p.ProductPrice - (p.ProductPrice * p.ProductSale / 100)) <= maxPrice.Value);
        if (onSaleOnly)
            products = products.Where(p => p.ProductSale > 0);

        // Apply sorting
        products = sortBy switch
        {
            "price-asc" => products.OrderBy(p => p.ProductPrice - (p.ProductPrice * p.ProductSale / 100)),
            "price-desc" => products.OrderByDescending(p => p.ProductPrice - (p.ProductPrice * p.ProductSale / 100)),
            "name" => products.OrderBy(p => p.ProductName),
            "sales" => products.OrderByDescending(p => p.QuantitySales),
            _ => products.OrderByDescending(p => p.ProductId)
        };

        var totalCount = products.Count();
        products = products.Skip((page - 1) * pageSize).Take(pageSize);

        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        ViewBag.TotalCount = totalCount;

        // Load categories and brands for filter
        ViewBag.Categories = await _unitOfWork.Categories.GetAllAsync();
        ViewBag.Brands = await _unitOfWork.Brands.GetAllAsync();

        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        // Load related products
        var relatedProducts = await _productService.GetProductsByCategoryAsync(product.ProductCategory);
        ViewBag.RelatedProducts = relatedProducts.Where(p => p.ProductId != id && p.ProductStatus == 1).Take(4);

        // Load product variants
        var variants = await _productService.GetProductVariantsAsync(id);
        ViewBag.Variants = variants.ToList();

        // Load reviews/evaluates
        var evaluates = await _unitOfWork.Evaluates.GetAllAsync();
        ViewBag.Evaluates = evaluates.Where(e => e.ProductId == id && e.EvaluateStatus == 1).OrderByDescending(e => e.EvaluateDate).ToList();

        return View(product);
    }

    [HttpGet]
    public IActionResult Search(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Index), new { searchTerm = term });
    }
}

