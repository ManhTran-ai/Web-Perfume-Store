using GuhaStore.Core.Entities;
using GuhaStore.Core.Interfaces;
using GuhaStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GuhaStore.Application.Services;

public class ArticleService : IArticleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public ArticleService(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<IEnumerable<Article>> GetAllArticlesAsync()
    {
        return await _context.Articles
            .OrderByDescending(a => a.ArticleDate)
            .ToListAsync();
    }

    public async Task<Article?> GetArticleByIdAsync(int id)
    {
        return await _context.Articles
            .Include(a => a.Comments.Where(c => c.CommentStatus == 1))
            .FirstOrDefaultAsync(a => a.ArticleId == id);
    }

    public async Task<IEnumerable<Article>> GetActiveArticlesAsync()
    {
        return await _context.Articles
            .Where(a => a.ArticleStatus == 1)
            .OrderByDescending(a => a.ArticleDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Article>> GetRecentArticlesAsync(int count)
    {
        return await _context.Articles
            .Where(a => a.ArticleStatus == 1)
            .OrderByDescending(a => a.ArticleDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Article> CreateArticleAsync(Article article)
    {
        article.ArticleDate = DateTime.Now;
        await _unitOfWork.Articles.AddAsync(article);
        await _unitOfWork.SaveChangesAsync();
        return article;
    }

    public async Task UpdateArticleAsync(Article article)
    {
        await _unitOfWork.Articles.UpdateAsync(article);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteArticleAsync(int id)
    {
        var article = await _unitOfWork.Articles.GetByIdAsync(id);
        if (article != null)
        {
            await _unitOfWork.Articles.DeleteAsync(article);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Comment>> GetArticleCommentsAsync(int articleId)
    {
        return await _context.Comments
            .Where(c => c.ArticleId == articleId && c.CommentStatus == 1)
            .OrderByDescending(c => c.CommentDate)
            .ToListAsync();
    }

    public async Task<Comment> AddCommentAsync(Comment comment)
    {
        comment.CommentDate = DateTime.Now;
        comment.CommentStatus = 0; // Pending by default
        await _unitOfWork.Comments.AddAsync(comment);
        await _unitOfWork.SaveChangesAsync();
        return comment;
    }
}

