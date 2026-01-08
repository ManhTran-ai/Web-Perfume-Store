using Microsoft.AspNetCore.Mvc;
using GuhaStore.Core.Interfaces;
using GuhaStore.Core.Entities;
using GuhaStore.Web.Models;
using Microsoft.AspNetCore.Http;

namespace GuhaStore.Web.Controllers;

public class CheckoutController : Controller
{
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public CheckoutController(
        IOrderService orderService,
        ICartService cartService,
        IUnitOfWork unitOfWork,
        IEmailService emailService)
    {
        _orderService = orderService;
        _cartService = cartService;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (accountId == null)
        {
            return RedirectToAction("Login", "Account", new { returnUrl = "/Checkout" });
        }

        var cartItems = await _cartService.GetCartItemsAsync();
        if (cartItems.Count == 0)
        {
            return RedirectToAction("Index", "Cart");
        }

        var totalAmount = await _cartService.GetCartTotalAsync();

        var model = new CheckoutViewModel
        {
            CartItems = cartItems,
            TotalAmount = totalAmount
        };

        // Load customer info if exists
        var customers = await _unitOfWork.Customers.GetAllAsync();
        var customer = customers.FirstOrDefault(c => c.AccountId == accountId);
        if (customer != null)
        {
            model.DeliveryName = customer.CustomerName;
            model.DeliveryPhone = customer.CustomerPhone;
            model.DeliveryAddress = customer.CustomerAddress;
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessOrder(CheckoutViewModel model)
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (accountId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            model.CartItems = await _cartService.GetCartItemsAsync();
            model.TotalAmount = await _cartService.GetCartTotalAsync();
            return View("Index", model);
        }

        // Validate cart inventory
        var isValid = await _orderService.ValidateCartInventoryAsync();
        if (!isValid)
        {
            ModelState.AddModelError("", "Một số sản phẩm trong giỏ hàng không còn đủ số lượng");
            model.CartItems = await _cartService.GetCartItemsAsync();
            model.TotalAmount = await _cartService.GetCartTotalAsync();
            return View("Index", model);
        }

        try
        {
            // Create delivery record
            var delivery = new Delivery
            {
                AccountId = accountId.Value,
                DeliveryName = model.DeliveryName,
                DeliveryPhone = model.DeliveryPhone,
                DeliveryAddress = model.DeliveryAddress,
                DeliveryNote = model.DeliveryNote
            };

            await _unitOfWork.Deliveries.AddAsync(delivery);
            await _unitOfWork.SaveChangesAsync();

            // Determine order type based on payment method
            int orderType;
            switch (model.PaymentMethod?.ToLower())
            {
                case "vnpay":
                    orderType = 2; // VNPay
                    break;
                case "momo":
                    orderType = 3; // MoMo
                    break;
                default:
                    orderType = 0; // COD
                    break;
            }

            // Create order
            var order = new Order
            {
                OrderCode = await _orderService.GenerateOrderCodeAsync(),
                OrderDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                AccountId = accountId.Value,
                DeliveryId = delivery.DeliveryId,
                TotalAmount = (int)model.TotalAmount,
                OrderType = orderType,
                OrderStatus = 0 // Pending
            };

            // Create order details
            var orderDetails = new List<OrderDetail>();
            var cartItems = await _cartService.GetCartItemsAsync();

            foreach (var item in cartItems)
            {
                var itemDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(
                    System.Text.Json.JsonSerializer.Serialize(item.Value));

                if (itemDict != null &&
                    itemDict.ContainsKey("ProductId") &&
                    itemDict.ContainsKey("CapacityId") &&
                    itemDict.ContainsKey("Quantity") &&
                    itemDict.ContainsKey("SalePrice"))
                {
                    var productId = Convert.ToInt32(itemDict["ProductId"].ToString());
                    var capacityId = Convert.ToInt32(itemDict["CapacityId"].ToString());
                    var quantity = Convert.ToInt32(itemDict["Quantity"].ToString());
                    var price = Convert.ToInt32(itemDict["SalePrice"].ToString());

                    var product = await _unitOfWork.Products.GetByIdAsync(productId);
                    if (product != null)
                    {
                        orderDetails.Add(new OrderDetail
                        {
                            OrderCode = order.OrderCode,
                            ProductId = productId,
                            CapacityId = capacityId,
                            ProductQuantity = quantity,
                            ProductPrice = price,
                            ProductSale = product.ProductSale
                        });
                    }
                }
            }

            // Create order with details
            await _orderService.CreateOrderAsync(order, orderDetails);

            // Handle payment based on method
            if (model.PaymentMethod?.ToLower() is "vnpay" or "momo")
            {
                // For online payments, redirect to payment page
                // Don't clear cart yet - it will be cleared after successful payment
                return RedirectToAction("Pay", "Payment", new { orderId = order.OrderId, provider = model.PaymentMethod });
            }
            else
            {
                // For COD, clear cart and send confirmation
                await _cartService.ClearCartAsync();
                await _emailService.SendOrderConfirmationAsync(order);
                return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
            }
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "Có lỗi xảy ra khi xử lý đơn hàng. Vui lòng thử lại.");
            model.CartItems = await _cartService.GetCartItemsAsync();
            model.TotalAmount = await _cartService.GetCartTotalAsync();
            return View("Index", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> OrderConfirmation(int orderId)
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

        return View(order);
    }
}

