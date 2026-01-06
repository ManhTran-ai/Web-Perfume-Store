namespace GuhaStore.Core.Entities;

public class Capacity
{
    public int CapacityId { get; set; }
    public string CapacityName { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}

