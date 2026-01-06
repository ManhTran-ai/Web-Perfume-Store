namespace GuhaStore.Core.Entities;

public class Delivery
{
    public int DeliveryId { get; set; }
    public int AccountId { get; set; }
    public string DeliveryName { get; set; } = string.Empty;
    public string DeliveryPhone { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public string DeliveryNote { get; set; } = string.Empty;
    
    // Navigation properties
    public Account? Account { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

