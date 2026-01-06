using GuhaStore.Application.Services;
using GuhaStore.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using GuhaStore.Core.Interfaces;

namespace GuhaStore.Web.Controllers;

public class BlogController : Controller
{
    private readonly IArticleService _articleService;

    public BlogController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 6)
    {
        var articles = await _articleService.GetActiveArticlesAsync();
        var totalCount = articles.Count();

        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        ViewBag.TotalCount = totalCount;

        var pagedArticles = articles.Skip((page - 1) * pageSize).Take(pageSize);
        return View(pagedArticles);
    }

    public async Task<IActionResult> Details(int id)
    {
        var article = await _articleService.GetArticleByIdAsync(id);
        if (article == null || article.ArticleStatus != 1)
        {
            return NotFound();
        }

        var comments = await _articleService.GetArticleCommentsAsync(id);
        var recentArticles = await _articleService.GetRecentArticlesAsync(5);

        ViewBag.Comments = comments;
        ViewBag.RecentArticles = recentArticles.Where(a => a.ArticleId != id);

        return View(article);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int articleId, string commentName, string commentEmail, string commentContent)
    {
        if (string.IsNullOrWhiteSpace(commentContent))
        {
            TempData["ErrorMessage"] = "Vui lòng nhập nội dung bình luận.";
            return RedirectToAction(nameof(Details), new { id = articleId });
        }

        var comment = new Comment
        {
            ArticleId = articleId,
            CommentName = commentName ?? "Khách",
            CommentEmail = commentEmail ?? string.Empty,
            CommentContent = commentContent,
            CommentDate = DateTime.Now,
            CommentStatus = 1
        };

        await _articleService.AddCommentAsync(comment);
        TempData["SuccessMessage"] = "Bình luận của bạn đã được gửi.";
        return RedirectToAction(nameof(Details), new { id = articleId });
    }
}

