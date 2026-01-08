using GuhaStore.Application.Services;
using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using GuhaStore.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace GuhaStore.Web.Areas.Admin.Controllers;

[Area("Admin")]
[SessionAuthorization(1, 2)] // Staff or Admin
public class OrderAdminController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IUnitOfWork _unitOfWork;

    public OrderAdminController(IOrderService orderService, IUnitOfWork unitOfWork)
    {
        _orderService = orderService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchTerm = "", int status = -1)
    {
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.SearchTerm = searchTerm;
        ViewBag.Status = status;

        var orders = await _unitOfWork.Orders.GetAllAsync();

        // Apply status filter
        if (status != -1)
        {
            orders = orders.Where(o => o.OrderStatus == status);
        }

        // Apply search filter
        if (!string.IsNullOrEmpty(searchTerm))
        {
            orders = orders.Where(o =>
                o.OrderCode.Contains(searchTerm) ||
                (o.Account != null && o.Account.AccountName.Contains(searchTerm)));
        }

        var totalCount = orders.Count();
        orders = orders.OrderByDescending(o => o.OrderId)
                      .Skip((page - 1) * pageSize)
                      .Take(pageSize);

        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        ViewBag.TotalCount = totalCount;

        return View(orders);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _orderService.GetOrderByIdAsync(id.Value);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Order order)
    {
        if (ModelState.IsValid)
        {
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order created successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View(order);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _unitOfWork.Orders.GetByIdAsync(id.Value);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Order order)
    {
        if (id != order.OrderId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int orderId, int newStatus)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
        if (order != null)
        {
            order.OrderStatus = newStatus;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Order status updated to {GetStatusName(newStatus)}.";
        }

        return RedirectToAction(nameof(Details), new { id = orderId });
    }

    private string GetStatusName(int status)
    {
        return status switch
        {
            0 => "Pending",
            1 => "Processing",
            2 => "Shipped",
            3 => "Delivered",
            4 => "Cancelled",
            _ => "Unknown"
        };
    }
}
