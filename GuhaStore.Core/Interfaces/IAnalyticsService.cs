namespace GuhaStore.Core.Interfaces;

public interface IAnalyticsService
{
    Task<SalesMetrics> GetSalesMetricsAsync(DateTime startDate, DateTime endDate);
    Task<OrderStatistics> GetOrderStatisticsAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<TopProduct>> GetTopProductsAsync(int count);
}

public class SalesMetrics
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int TotalItemsSold { get; set; }
    public decimal AverageOrderValue { get; set; }
}

public class OrderStatistics
{
    public int PendingOrders { get; set; }
    public int ProcessingOrders { get; set; }
    public int ShippedOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public int CancelledOrders { get; set; }
}

public class TopProduct
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}

