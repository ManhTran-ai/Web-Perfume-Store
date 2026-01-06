namespace GuhaStore.Core.Entities;

public class ProductVariant
{
    public int VariantId { get; set; }
    public int ProductId { get; set; }
    public int CapacityId { get; set; }
    public int? VariantPrice { get; set; }
    public int VariantQuantity { get; set; }
    public bool VariantStatus { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation properties
    public Product? Product { get; set; }
    public Capacity? Capacity { get; set; }
}

