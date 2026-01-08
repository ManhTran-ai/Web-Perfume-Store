namespace GuhaStore.Core.Interfaces;

public interface IPaymentService
{
    Task<string> CreatePaymentUrlAsync(int orderCode, decimal amount, string returnUrl, string ipAddress);
    Task<PaymentResult> ProcessCallbackAsync(IDictionary<string, string> parameters);
    Task<bool> VerifyPaymentAsync(string transactionId);
    string GetProviderName();
}

public class PaymentResult
{
    public bool IsSuccess { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string OrderCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
