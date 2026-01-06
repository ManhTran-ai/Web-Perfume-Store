using GuhaStore.Core.Entities;

namespace GuhaStore.Core.Interfaces;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(Order order, List<OrderDetail> orderDetails);
    Task<Order?> GetOrderByIdAsync(int orderId);
    Task<Order?> GetOrderByCodeAsync(int orderCode);
    Task<IEnumerable<Order>> GetUserOrdersAsync(int accountId);
    Task UpdateOrderStatusAsync(int orderId, int status);
    Task<bool> ValidateCartInventoryAsync();
    Task<int> GenerateOrderCodeAsync();
}

