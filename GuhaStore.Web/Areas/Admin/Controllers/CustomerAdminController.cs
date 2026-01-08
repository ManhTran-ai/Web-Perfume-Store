using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using GuhaStore.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace GuhaStore.Web.Areas.Admin.Controllers;

[Area("Admin")]
[SessionAuthorization(1, 2)] // Staff or Admin
public class CustomerAdminController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerAdminController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchTerm = "")
    {
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.SearchTerm = searchTerm;

        var customers = await _unitOfWork.Customers.GetAllAsync();

        // Apply search filter
        if (!string.IsNullOrEmpty(searchTerm))
        {
            customers = customers.Where(c =>
                c.CustomerName.Contains(searchTerm) ||
                c.CustomerEmail.Contains(searchTerm) ||
                c.CustomerPhone.Contains(searchTerm));
        }

        var totalCount = customers.Count();
        customers = customers.OrderByDescending(c => c.CustomerId)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize);

        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        ViewBag.TotalCount = totalCount;

        return View(customers);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var customer = await _unitOfWork.Customers.GetByIdAsync(id.Value);
        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (ModelState.IsValid)
        {
            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Customer created successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View(customer);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var customer = await _unitOfWork.Customers.GetByIdAsync(id.Value);
        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer customer)
    {
        if (id != customer.CustomerId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Customer updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        return View(customer);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var customer = await _unitOfWork.Customers.GetByIdAsync(id.Value);
        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(id);
        if (customer != null)
        {
            _unitOfWork.Customers.Remove(customer);
            await _unitOfWork.SaveChangesAsync();
        }

        TempData["SuccessMessage"] = "Customer deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
