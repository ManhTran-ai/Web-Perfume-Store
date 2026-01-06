namespace GuhaStore.Core.Entities;

public class OrderDetail
{
    public int OrderDetailId { get; set; }
    public int OrderCode { get; set; }
    public int ProductId { get; set; }
    public int CapacityId { get; set; } // Optional - may not exist in all database versions
    public int ProductQuantity { get; set; }
    public int ProductPrice { get; set; }
    public int ProductSale { get; set; }
    
    // Navigation properties
    public Order? Order { get; set; }
    public Product? Product { get; set; }
    public Capacity? Capacity { get; set; }
}

