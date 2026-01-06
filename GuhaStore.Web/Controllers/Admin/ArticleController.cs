using GuhaStore.Application.Services;
using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using GuhaStore.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace GuhaStore.Web.Controllers.Admin;

[SessionAuthorization(1, 2)] // Staff or Admin
public class ArticleController : Controller
{
    private readonly IArticleService _articleService;
    private readonly IFileUploadService _fileUploadService;

    public ArticleController(IArticleService articleService, IFileUploadService fileUploadService)
    {
        _articleService = articleService;
        _fileUploadService = fileUploadService;
    }

    public async Task<IActionResult> Index()
    {
        var articles = await _articleService.GetAllArticlesAsync();
        return View(articles.OrderByDescending(a => a.ArticleDate));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var article = await _articleService.GetArticleByIdAsync(id.Value);
        if (article == null)
        {
            return NotFound();
        }

        return View(article);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Article article, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                var imagePath = await _fileUploadService.UploadArticleImageAsync(stream, imageFile.FileName);
                article.ArticleImage = imagePath;
            }

            await _articleService.CreateArticleAsync(article);
            TempData["SuccessMessage"] = "Tạo bài viết thành công.";
            return RedirectToAction(nameof(Index));
        }

        return View(article);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var article = await _articleService.GetArticleByIdAsync(id.Value);
        if (article == null)
        {
            return NotFound();
        }

        return View(article);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Article article, IFormFile? imageFile)
    {
        if (id != article.ArticleId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                var imagePath = await _fileUploadService.UploadArticleImageAsync(stream, imageFile.FileName);
                article.ArticleImage = imagePath;
            }

            await _articleService.UpdateArticleAsync(article);
            TempData["SuccessMessage"] = "Cập nhật bài viết thành công.";
            return RedirectToAction(nameof(Index));
        }

        return View(article);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var article = await _articleService.GetArticleByIdAsync(id.Value);
        if (article == null)
        {
            return NotFound();
        }

        return View(article);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _articleService.DeleteArticleAsync(id);
        TempData["SuccessMessage"] = "Xóa bài viết thành công.";
        return RedirectToAction(nameof(Index));
    }
}

