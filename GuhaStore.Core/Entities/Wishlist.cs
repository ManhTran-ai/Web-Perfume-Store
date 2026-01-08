namespace GuhaStore.Core.Entities;

public class Wishlist
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public int ProductId { get; set; }
    public DateTime AddedAt { get; set; }

    // Navigation properties
    public Account? Account { get; set; }
    public Product? Product { get; set; }
}
