namespace GuhaStore.Core.Entities;

public class PaymentTransaction
{
    public int Id { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public int OrderCode { get; set; }
    public string Provider { get; set; } = string.Empty; // VNPay, MoMo, etc.
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty; // Pending, Success, Failed
    public string ProviderPayload { get; set; } = string.Empty; // JSON response from provider
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Order? Order { get; set; }
}
