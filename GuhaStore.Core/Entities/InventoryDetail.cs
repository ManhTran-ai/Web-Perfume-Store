namespace GuhaStore.Core.Entities;

public class InventoryDetail
{
    public int InventoryDetailId { get; set; }
    public string InventoryCode { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public int ProductQuantity { get; set; }
    public decimal ProductPriceImport { get; set; }
    
    // Navigation properties
    public Inventory? Inventory { get; set; }
    public Product? Product { get; set; }
}

