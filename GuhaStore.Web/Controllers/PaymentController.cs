using GuhaStore.Application.Services;
using GuhaStore.Application.Services.Payment;
using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using GuhaStore.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace GuhaStore.Web.Controllers;

public class PaymentController : Controller
{
    private readonly VnPayService _vnPayService;
    private readonly MoMoService _moMoService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderService _orderService;

    public PaymentController(
        VnPayService vnPayService,
        MoMoService moMoService,
        IUnitOfWork unitOfWork,
        IOrderService orderService)
    {
        _vnPayService = vnPayService;
        _moMoService = moMoService;
        _unitOfWork = unitOfWork;
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> Pay(int orderId, string provider)
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (accountId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null || order.AccountId != accountId)
        {
            return NotFound();
        }

        if (order.OrderStatus != 0) // Not pending
        {
            TempData["ErrorMessage"] = "Order is not in pending status for payment.";
            return RedirectToAction("OrderDetail", "Account", new { orderId });
        }

        IPaymentService paymentService;
        switch (provider.ToLower())
        {
            case "vnpay":
                paymentService = _vnPayService;
                order.OrderType = 2; // VNPay
                break;
            case "momo":
                paymentService = _moMoService;
                order.OrderType = 3; // MoMo
                break;
            default:
                TempData["ErrorMessage"] = "Invalid payment provider.";
                return RedirectToAction("OrderDetail", "Account", new { orderId });
        }

        // Update order type
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();

        // Create payment transaction record
        var transaction = new PaymentTransaction
        {
            TransactionId = Guid.NewGuid().ToString(),
            OrderCode = order.OrderCode,
            Provider = paymentService.GetProviderName(),
            Amount = order.TotalAmount,
            Status = "Pending",
            CreatedAt = DateTime.Now
        };

        await _unitOfWork.PaymentTransactions.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();

        // Get client IP
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

        try
        {
            var paymentUrl = await paymentService.CreatePaymentUrlAsync(
                order.OrderCode,
                order.TotalAmount,
                Url.Action("Return", "Payment", null, Request.Scheme),
                ipAddress
            );

            return Redirect(paymentUrl);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Payment creation failed: {ex.Message}";
            return RedirectToAction("OrderDetail", "Account", new { orderId });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Return()
    {
        var provider = Request.Query["provider"].ToString() ?? "vnpay";
        var parameters = Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());

        IPaymentService paymentService;
        switch (provider.ToLower())
        {
            case "vnpay":
                paymentService = _vnPayService;
                break;
            case "momo":
                paymentService = _moMoService;
                break;
            default:
                TempData["ErrorMessage"] = "Invalid payment provider.";
                return RedirectToAction("Index", "Home");
        }

        try
        {
            var result = await paymentService.ProcessCallbackAsync(parameters);

            if (result.IsSuccess)
            {
                // Update payment transaction
                var transaction = await _unitOfWork.PaymentTransactions
                    .FirstOrDefaultAsync(pt => pt.OrderCode.ToString() == result.OrderCode && pt.Status == "Pending");

                if (transaction != null)
                {
                    transaction.Status = "Success";
                    transaction.TransactionId = result.TransactionId;
                    transaction.UpdatedAt = DateTime.Now;
                    _unitOfWork.PaymentTransactions.Update(transaction);
                }

                // Update order status to processing
                if (int.TryParse(result.OrderCode, out var orderCode))
                {
                    var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
                    if (order != null)
                    {
                        order.OrderStatus = 1; // Processing
                        _unitOfWork.Orders.Update(order);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                TempData["SuccessMessage"] = "Payment successful! Your order is now being processed.";
                return RedirectToAction("OrderConfirmation", "Checkout", new { orderId = result.OrderCode });
            }
            else
            {
                // Update payment transaction as failed
                var transaction = await _unitOfWork.PaymentTransactions
                    .FirstOrDefaultAsync(pt => pt.OrderCode.ToString() == result.OrderCode && pt.Status == "Pending");

                if (transaction != null)
                {
                    transaction.Status = "Failed";
                    transaction.UpdatedAt = DateTime.Now;
                    _unitOfWork.PaymentTransactions.Update(transaction);
                    await _unitOfWork.SaveChangesAsync();
                }

                TempData["ErrorMessage"] = $"Payment failed: {result.Message}";
                return RedirectToAction("Index", "Home");
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Payment processing error: {ex.Message}";
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Notify()
    {
        // Handle IPN (Instant Payment Notification) from payment providers
        // This endpoint is called by the payment provider server-side
        var provider = Request.Form["provider"].ToString() ?? "vnpay";
        var parameters = Request.Form.ToDictionary(f => f.Key, f => f.Value.ToString());

        IPaymentService paymentService;
        switch (provider.ToLower())
        {
            case "vnpay":
                paymentService = _vnPayService;
                break;
            case "momo":
                paymentService = _moMoService;
                break;
            default:
                return BadRequest("Invalid provider");
        }

        try
        {
            var result = await paymentService.ProcessCallbackAsync(parameters);

            if (result.IsSuccess)
            {
                // Update payment transaction
                var transaction = await _unitOfWork.PaymentTransactions
                    .FirstOrDefaultAsync(pt => pt.OrderCode.ToString() == result.OrderCode);

                if (transaction != null && transaction.Status != "Success")
                {
                    transaction.Status = "Success";
                    transaction.TransactionId = result.TransactionId;
                    transaction.UpdatedAt = DateTime.Now;
                    _unitOfWork.PaymentTransactions.Update(transaction);

                    // Update order status
                    if (int.TryParse(result.OrderCode, out var orderCode))
                    {
                        var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
                        if (order != null && order.OrderStatus == 0)
                        {
                            order.OrderStatus = 1; // Processing
                            _unitOfWork.Orders.Update(order);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();
                }
            }

            return Ok("OK");
        }
        catch
        {
            return BadRequest("Error processing notification");
        }
    }
}
