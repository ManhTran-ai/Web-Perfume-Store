using System.Diagnostics;
using GuhaStore.Application.Services;
using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GuhaStore.Web.Models;

namespace GuhaStore.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;
    private readonly IArticleService _articleService;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(
        ILogger<HomeController> logger,
        IProductService productService,
        IArticleService articleService,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _productService = productService;
        _articleService = articleService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        // Load featured/new products
        var products = await _productService.GetActiveProductsAsync();
        ViewBag.NewProducts = products.OrderByDescending(p => p.ProductId).Take(8);
        ViewBag.BestSellers = products.OrderByDescending(p => p.QuantitySales).Take(8);
        ViewBag.SaleProducts = products.Where(p => p.ProductSale > 0).OrderByDescending(p => p.ProductSale).Take(8);

        // Load categories
        ViewBag.Categories = await _unitOfWork.Categories.GetAllAsync();

        // Load brands
        ViewBag.Brands = await _unitOfWork.Brands.GetAllAsync();

        // Load recent articles
        ViewBag.RecentArticles = await _articleService.GetRecentArticlesAsync(3);

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
