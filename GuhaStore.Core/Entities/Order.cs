namespace GuhaStore.Core.Entities;

public class Order
{
    public int OrderId { get; set; }
    public int OrderCode { get; set; }
    public string OrderDate { get; set; } = string.Empty;
    public int AccountId { get; set; }
    public int DeliveryId { get; set; }
    public int TotalAmount { get; set; }
    public int OrderType { get; set; } // 0=COD, 1=Online, 2=VNPay, 3=MoMo, 4=Other, 5=Direct
    public int OrderStatus { get; set; } // 0=Pending, 1=Processing, 2=Shipped, 3=Delivered, 4=Cancelled, -1=Failed
    
    // Navigation properties
    public Account? Account { get; set; }
    public Delivery? Delivery { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}

