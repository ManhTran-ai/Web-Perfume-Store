using GuhaStore.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace GuhaStore.Application.Services.Payment;

public class VnPayService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly string _tmnCode;
    private readonly string _hashSecret;
    private readonly string _url;
    private readonly string _returnUrl;

    public VnPayService(IConfiguration configuration)
    {
        _configuration = configuration;
        _tmnCode = _configuration["PaymentSettings:VnPay:TmnCode"] ?? throw new ArgumentNullException("VnPay TmnCode not configured");
        _hashSecret = _configuration["PaymentSettings:VnPay:HashSecret"] ?? throw new ArgumentNullException("VnPay HashSecret not configured");
        _url = _configuration["PaymentSettings:VnPay:Url"] ?? throw new ArgumentNullException("VnPay Url not configured");
        _returnUrl = _configuration["PaymentSettings:VnPay:ReturnUrl"] ?? throw new ArgumentNullException("VnPay ReturnUrl not configured");
    }

    public string GetProviderName() => "VNPay";

    public async Task<string> CreatePaymentUrlAsync(int orderCode, decimal amount, string returnUrl, string ipAddress)
    {
        var vnpayParams = new SortedDictionary<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", _tmnCode },
            { "vnp_Amount", ((int)(amount * 100)).ToString() }, // VNPay expects amount in smallest currency unit
            { "vnp_CurrCode", "VND" },
            { "vnp_TxnRef", orderCode.ToString() },
            { "vnp_OrderInfo", $"Thanh toan don hang {orderCode}" },
            { "vnp_OrderType", "other" },
            { "vnp_Locale", "vn" },
            { "vnp_ReturnUrl", _returnUrl },
            { "vnp_IpAddr", ipAddress },
            { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }
        };

        var signData = string.Join("&", vnpayParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        var secureHash = HmacSHA512(_hashSecret, signData);
        vnpayParams.Add("vnp_SecureHash", secureHash);

        var paymentUrl = $"{_url}?{string.Join("&", vnpayParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))}";

        return await Task.FromResult(paymentUrl);
    }

    public async Task<PaymentResult> ProcessCallbackAsync(IDictionary<string, string> parameters)
    {
        var result = new PaymentResult();

        // Verify checksum
        var secureHash = parameters["vnp_SecureHash"];
        parameters.Remove("vnp_SecureHash");
        parameters.Remove("vnp_SecureHashType");

        var signData = string.Join("&", parameters.OrderBy(kvp => kvp.Key).Select(kvp => $"{kvp.Key}={kvp.Value}"));
        var checkSum = HmacSHA512(_hashSecret, signData);

        if (!secureHash.Equals(checkSum, StringComparison.InvariantCultureIgnoreCase))
        {
            result.IsSuccess = false;
            result.Message = "Invalid checksum";
            return result;
        }

        result.TransactionId = parameters["vnp_TransactionNo"];
        result.OrderCode = parameters["vnp_TxnRef"];

        if (int.TryParse(parameters["vnp_Amount"], out var amount))
        {
            result.Amount = amount / 100m; // Convert back from smallest currency unit
        }

        var responseCode = parameters["vnp_ResponseCode"];
        var transactionStatus = parameters["vnp_TransactionStatus"];

        result.IsSuccess = responseCode == "00" && transactionStatus == "00";
        result.Message = result.IsSuccess ? "Payment successful" : $"Payment failed: {responseCode}";

        return await Task.FromResult(result);
    }

    public async Task<bool> VerifyPaymentAsync(string transactionId)
    {
        // In a real implementation, you would query VNPay's API to verify the transaction
        // For sandbox/demo purposes, we'll return true
        return await Task.FromResult(true);
    }

    private static string HmacSHA512(string key, string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);

        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
