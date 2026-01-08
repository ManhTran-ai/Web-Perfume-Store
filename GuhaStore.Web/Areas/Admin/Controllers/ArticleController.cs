using GuhaStore.Application.Services;
using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using GuhaStore.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace GuhaStore.Web.Areas.Admin.Controllers;

[Area("Admin")]
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

    // Comment Management
    public async Task<IActionResult> Comments(int page = 1, int pageSize = 10, int status = -1)
    {
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.Status = status;

        var comments = await _unitOfWork.Comments.GetAllAsync();

        // Apply status filter
        if (status != -1)
        {
            comments = comments.Where(c => c.CommentStatus == status);
        }

        var totalCount = comments.Count();
        comments = comments.OrderByDescending(c => c.CommentId)
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize);

        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        ViewBag.TotalCount = totalCount;

        return View(comments);
    }

    [HttpPost]
    public async Task<IActionResult> ApproveComment(int commentId)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
        if (comment != null)
        {
            comment.CommentStatus = 1; // Approved
            _unitOfWork.Comments.Update(comment);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Comment approved successfully.";
        }

        return RedirectToAction(nameof(Comments));
    }

    [HttpPost]
    public async Task<IActionResult> RejectComment(int commentId)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
        if (comment != null)
        {
            comment.CommentStatus = 2; // Rejected
            _unitOfWork.Comments.Update(comment);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Comment rejected.";
        }

        return RedirectToAction(nameof(Comments));
    }
}

