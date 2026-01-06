namespace GuhaStore.Core.Entities;

public class Customer
{
    public int CustomerId { get; set; }
    public int AccountId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int CustomerGender { get; set; } // 0=Unknown, 1=Male, 2=Female
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerAddress { get; set; } = string.Empty;
    
    // Navigation properties
    public Account? Account { get; set; }
}

