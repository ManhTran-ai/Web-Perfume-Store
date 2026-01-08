using GuhaStore.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace GuhaStore.Application.Services.Payment;

public class MoMoService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly string _partnerCode;
    private readonly string _accessKey;
    private readonly string _secretKey;
    private readonly string _endpoint;
    private readonly string _returnUrl;
    private readonly string _notifyUrl;

    public MoMoService(IConfiguration configuration)
    {
        _configuration = configuration;
        _partnerCode = _configuration["PaymentSettings:MoMo:PartnerCode"] ?? throw new ArgumentNullException("MoMo PartnerCode not configured");
        _accessKey = _configuration["PaymentSettings:MoMo:AccessKey"] ?? throw new ArgumentNullException("MoMo AccessKey not configured");
        _secretKey = _configuration["PaymentSettings:MoMo:SecretKey"] ?? throw new ArgumentNullException("MoMo SecretKey not configured");
        _endpoint = _configuration["PaymentSettings:MoMo:Endpoint"] ?? throw new ArgumentNullException("MoMo Endpoint not configured");
        _returnUrl = _configuration["PaymentSettings:MoMo:ReturnUrl"] ?? throw new ArgumentNullException("MoMo ReturnUrl not configured");
        _notifyUrl = _configuration["PaymentSettings:MoMo:IpnUrl"] ?? throw new ArgumentNullException("MoMo IpnUrl not configured");
    }

    public string GetProviderName() => "MoMo";

    public async Task<string> CreatePaymentUrlAsync(int orderCode, decimal amount, string returnUrl, string ipAddress)
    {
        var requestId = Guid.NewGuid().ToString();
        var orderId = orderCode.ToString();
        var requestType = "payWithATM";

        var rawData = $"accessKey={_accessKey}&amount={(int)amount}&extraData=&ipnUrl={_notifyUrl}&orderId={orderId}&orderInfo=Thanh toan don hang {orderCode}&partnerCode={_partnerCode}&redirectUrl={_returnUrl}&requestId={requestId}&requestType={requestType}";
        var signature = SignHmacSHA256(rawData, _secretKey);

        var requestData = new
        {
            partnerCode = _partnerCode,
            accessKey = _accessKey,
            requestId = requestId,
            amount = (int)amount,
            orderId = orderId,
            orderInfo = $"Thanh toan don hang {orderCode}",
            redirectUrl = _returnUrl,
            ipnUrl = _notifyUrl,
            extraData = "",
            requestType = requestType,
            signature = signature,
            lang = "vi"
        };

        using var client = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(_endpoint, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        var responseData = JsonSerializer.Deserialize<MoMoResponse>(responseContent);
        if (responseData?.resultCode == 0)
        {
            return responseData.payUrl;
        }

        throw new Exception($"MoMo payment creation failed: {responseData?.message ?? "Unknown error"}");
    }

    public async Task<PaymentResult> ProcessCallbackAsync(IDictionary<string, string> parameters)
    {
        var result = new PaymentResult();

        // Verify signature
        var signature = parameters["signature"];
        var rawData = $"accessKey={_accessKey}&amount={parameters["amount"]}&extraData={parameters["extraData"]}&message={parameters["message"]}&orderId={parameters["orderId"]}&orderInfo={parameters["orderInfo"]}&orderType={parameters["orderType"]}&partnerCode={parameters["partnerCode"]}&payType={parameters["payType"]}&requestId={parameters["requestId"]}&responseTime={parameters["responseTime"]}&resultCode={parameters["resultCode"]}&transId={parameters["transId"]}";
        var expectedSignature = SignHmacSHA256(rawData, _secretKey);

        if (!signature.Equals(expectedSignature, StringComparison.InvariantCultureIgnoreCase))
        {
            result.IsSuccess = false;
            result.Message = "Invalid signature";
            return result;
        }

        result.TransactionId = parameters["transId"];
        result.OrderCode = parameters["orderId"];

        if (int.TryParse(parameters["amount"], out var amount))
        {
            result.Amount = amount;
        }

        var resultCode = parameters["resultCode"];
        result.IsSuccess = resultCode == "0";
        result.Message = result.IsSuccess ? "Payment successful" : $"Payment failed: {parameters["message"]}";

        return await Task.FromResult(result);
    }

    public async Task<bool> VerifyPaymentAsync(string transactionId)
    {
        // In a real implementation, you would query MoMo's API to verify the transaction
        // For sandbox/demo purposes, we'll return true
        return await Task.FromResult(true);
    }

    private static string SignHmacSHA256(string data, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);

        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    private class MoMoResponse
    {
        public int resultCode { get; set; }
        public string message { get; set; } = string.Empty;
        public string payUrl { get; set; } = string.Empty;
    }
}
