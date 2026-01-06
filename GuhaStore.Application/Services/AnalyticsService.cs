using GuhaStore.Core.Interfaces;
using GuhaStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GuhaStore.Application.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly ApplicationDbContext _context;

    public AnalyticsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SalesMetrics> GetSalesMetricsAsync(DateTime startDate, DateTime endDate)
    {
        var allOrders = await _context.Orders.ToListAsync();
        var orders = allOrders
            .Where(o => DateTime.TryParse(o.OrderDate, out DateTime orderDate) &&
                orderDate >= startDate && orderDate <= endDate &&
                o.OrderStatus == 3) // Delivered
            .ToList();

        var totalRevenue = orders.Sum(o => (decimal)o.TotalAmount);
        var totalOrders = orders.Count;
        var totalItemsSold = await _context.OrderDetails
            .Where(od => orders.Select(o => o.OrderCode).Contains(od.OrderCode))
            .SumAsync(od => od.ProductQuantity);

        return new SalesMetrics
        {
            TotalRevenue = totalRevenue,
            TotalOrders = totalOrders,
            TotalItemsSold = totalItemsSold,
            AverageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0
        };
    }

    public async Task<OrderStatistics> GetOrderStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        var allOrders = await _context.Orders.ToListAsync();
        var orders = allOrders
            .Where(o => DateTime.TryParse(o.OrderDate, out DateTime orderDate) &&
                orderDate >= startDate && orderDate <= endDate)
            .ToList();

        return new OrderStatistics
        {
            PendingOrders = orders.Count(o => o.OrderStatus == 0),
            ProcessingOrders = orders.Count(o => o.OrderStatus == 1),
            ShippedOrders = orders.Count(o => o.OrderStatus == 2),
            DeliveredOrders = orders.Count(o => o.OrderStatus == 3),
            CancelledOrders = orders.Count(o => o.OrderStatus == 4)
        };
    }

    public async Task<IEnumerable<TopProduct>> GetTopProductsAsync(int count)
    {
        var topProducts = await _context.OrderDetails
            .Include(od => od.Product)
            .GroupBy(od => new { od.ProductId, od.Product!.ProductName })
            .Select(g => new TopProduct
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.ProductName,
                QuantitySold = g.Sum(od => od.ProductQuantity),
                Revenue = g.Sum(od => (decimal)(od.ProductPrice * od.ProductQuantity * (100 - od.ProductSale) / 100))
            })
            .OrderByDescending(p => p.QuantitySold)
            .Take(count)
            .ToListAsync();

        return topProducts;
    }
}

