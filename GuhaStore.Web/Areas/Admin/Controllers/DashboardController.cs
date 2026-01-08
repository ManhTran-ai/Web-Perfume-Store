using GuhaStore.Application.Services;
using GuhaStore.Core.Interfaces;
using GuhaStore.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace GuhaStore.Web.Areas.Admin.Controllers;

[Area("Admin")]
[SessionAuthorization(1, 2)] // Staff or Admin
public class DashboardController : Controller
{
    private readonly IAnalyticsService _analyticsService;
    private readonly IUnitOfWork _unitOfWork;

    public DashboardController(IAnalyticsService analyticsService, IUnitOfWork unitOfWork)
    {
        _analyticsService = analyticsService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        // Get current month metrics
        var now = DateTime.Now;
        var startDate = new DateTime(now.Year, now.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var salesMetrics = await _analyticsService.GetSalesMetricsAsync(startDate, endDate);
        var orderStatistics = await _analyticsService.GetOrderStatisticsAsync(startDate, endDate);
        var topProducts = await _analyticsService.GetTopProductsAsync(5);

        ViewBag.SalesMetrics = salesMetrics;
        ViewBag.OrderStatistics = orderStatistics;
        ViewBag.TopProducts = topProducts;

        // Get recent orders for quick overview
        var recentOrders = await _unitOfWork.Orders.GetAllAsync();
        ViewBag.RecentOrders = recentOrders
            .OrderByDescending(o => o.OrderId)
            .Take(10)
            .ToList();

        return View();
    }
}
