using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using GuhaStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GuhaStore.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;
    private readonly ICartService _cartService;

    public OrderService(IUnitOfWork unitOfWork, ApplicationDbContext context, ICartService cartService)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _cartService = cartService;
    }

    public async Task<Order> CreateOrderAsync(Order order, List<OrderDetail> orderDetails)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Generate order code if not set
            if (order.OrderCode == 0)
            {
                order.OrderCode = await GenerateOrderCodeAsync();
            }

            // Set order date
            order.OrderDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // Add order
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // Add order details and update inventory
            foreach (var detail in orderDetails)
            {
                detail.OrderCode = order.OrderCode;
                await _unitOfWork.OrderDetails.AddAsync(detail);

                // Update product variant quantity
                var variant = await _context.ProductVariants
                    .FirstOrDefaultAsync(pv => pv.ProductId == detail.ProductId && 
                        pv.CapacityId == detail.CapacityId);

                if (variant != null)
                {
                    variant.VariantQuantity -= detail.ProductQuantity;
                    variant.UpdatedAt = DateTime.Now;
                    _context.ProductVariants.Update(variant);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return order;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        return await _context.Orders
            .Include(o => o.Account)
            .Include(o => o.Delivery)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Capacity)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task<Order?> GetOrderByCodeAsync(int orderCode)
    {
        return await _context.Orders
            .Include(o => o.Account)
            .Include(o => o.Delivery)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Capacity)
            .FirstOrDefaultAsync(o => o.OrderCode == orderCode);
    }

    public async Task<IEnumerable<Order>> GetUserOrdersAsync(int accountId)
    {
        return await _context.Orders
            .Include(o => o.Delivery)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .Where(o => o.AccountId == accountId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task UpdateOrderStatusAsync(int orderId, int status)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
        if (order != null)
        {
            order.OrderStatus = status;
            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<bool> ValidateCartInventoryAsync()
    {
        return await _cartService.ValidateCartAsync();
    }

    public async Task<int> GenerateOrderCodeAsync()
    {
        var random = new Random();
        int orderCode;
        bool exists;

        do
        {
            orderCode = random.Next(1000, 9999);
            exists = await _context.Orders.AnyAsync(o => o.OrderCode == orderCode);
        } while (exists);

        return orderCode;
    }
}

