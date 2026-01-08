namespace GuhaStore.Core.Entities;

public class Order
{
    public int OrderId { get; set; }
    public int OrderCode { get; set; }
    public int AccountId { get; set; }
    public DateTime OrderDate { get; set; }
    public int TotalAmount { get; set; }
    public int OrderStatus { get; set; } // 0=Pending, 1=Processing, 2=Shipped, 3=Delivered, 4=Cancelled
    // Delivery info embedded to simplify schema
    public string? DeliveryName { get; set; }
    public string? DeliveryPhone { get; set; }
    public string? DeliveryAddress { get; set; }
    
    // Navigation properties
    public Account? Account { get; set; }
    public Delivery? Delivery { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}

