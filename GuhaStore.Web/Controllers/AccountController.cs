using Microsoft.AspNetCore.Mvc;
using GuhaStore.Core.Interfaces;
using GuhaStore.Core.Entities;
using GuhaStore.Web.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace GuhaStore.Web.Controllers;

public class AccountController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AccountController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var accounts = await _unitOfWork.Accounts.GetAllAsync();
        var account = accounts.FirstOrDefault(a => a.AccountName == model.Username && a.AccountStatus == 0);

        if (account == null || !BCrypt.Net.BCrypt.Verify(model.Password, account.AccountPassword))
        {
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
            return View(model);
        }

        // Store user info in session
        HttpContext.Session.SetInt32("AccountId", account.AccountId);
        HttpContext.Session.SetString("AccountName", account.AccountName);
        HttpContext.Session.SetInt32("AccountType", account.AccountType);
        HttpContext.Session.SetString("AccountEmail", account.AccountEmail);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        // Redirect based on account type
        if (account.AccountType == 2) // Admin
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }
        else if (account.AccountType == 1) // Staff
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Check if username already exists
        var accounts = await _unitOfWork.Accounts.GetAllAsync();
        var existingAccount = accounts.FirstOrDefault(a => a.AccountName == model.Username || a.AccountEmail == model.Email);

        if (existingAccount != null)
        {
            ModelState.AddModelError("", "Tên đăng nhập hoặc email đã tồn tại");
            return View(model);
        }

        // Create new account
        var account = new Account
        {
            AccountName = model.Username,
            AccountEmail = model.Email,
            AccountPassword = BCrypt.Net.BCrypt.HashPassword(model.Password),
            AccountPhone = model.Phone,
            AccountType = 0, // Customer
            AccountStatus = 0 // Active
        };

        await _unitOfWork.Accounts.AddAsync(account);
        await _unitOfWork.SaveChangesAsync();

        // Auto login after registration
        HttpContext.Session.SetInt32("AccountId", account.AccountId);
        HttpContext.Session.SetString("AccountName", account.AccountName);
        HttpContext.Session.SetInt32("AccountType", account.AccountType);
        HttpContext.Session.SetString("AccountEmail", account.AccountEmail);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult MyAccount()
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (accountId == null)
        {
            return RedirectToAction("Login");
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> OrderHistory()
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (accountId == null)
        {
            return RedirectToAction("Login");
        }

        var orderService = HttpContext.RequestServices.GetRequiredService<IOrderService>();
        var orders = await orderService.GetUserOrdersAsync(accountId.Value);
        return View(orders);
    }

    [HttpGet]
    public async Task<IActionResult> OrderDetail(int orderId)
    {
        var accountId = HttpContext.Session.GetInt32("AccountId");
        if (accountId == null)
        {
            return RedirectToAction("Login");
        }

        var orderService = HttpContext.RequestServices.GetRequiredService<IOrderService>();
        var order = await orderService.GetOrderByIdAsync(orderId);
        
        if (order == null || order.AccountId != accountId)
        {
            return NotFound();
        }

        return View(order);
    }
}

