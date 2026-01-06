namespace GuhaStore.Core.Entities;

public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryDescription { get; set; } = string.Empty;
    public string CategoryImage { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

