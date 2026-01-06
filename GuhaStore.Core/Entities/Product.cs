namespace GuhaStore.Core.Entities;

public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int ProductCategory { get; set; } // Maps to category_id
    public int ProductBrand { get; set; } // Maps to brand_id
    public int CapacityId { get; set; } // Legacy field, kept for backward compatibility
    public int ProductQuantity { get; set; }
    public int QuantitySales { get; set; }
    public int ProductPriceImport { get; set; }
    public int ProductPrice { get; set; }
    public int ProductSale { get; set; } // Sale percentage
    public string ProductDescription { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public int ProductStatus { get; set; } // 0=Inactive, 1=Active
    
    // Navigation properties
    public Category? Category { get; set; }
    public Brand? Brand { get; set; }
    public Capacity? Capacity { get; set; }
    public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public ICollection<Evaluate> Evaluates { get; set; } = new List<Evaluate>();
}

