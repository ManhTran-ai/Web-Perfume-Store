using Microsoft.AspNetCore.Mvc;

namespace GuhaStore.Web.Controllers;

public class ContactController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Submit(string name, string email, string phone, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            TempData["ErrorMessage"] = "Vui lòng nhập nội dung tin nhắn.";
            return RedirectToAction(nameof(Index));
        }

        // In a real application, you would send an email here
        // For now, just show success message
        TempData["SuccessMessage"] = "Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi sớm nhất có thể.";
        return RedirectToAction(nameof(Index));
    }
}

