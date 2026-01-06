using GuhaStore.Core.Entities;

namespace GuhaStore.Core.Interfaces;

public interface IArticleService
{
    Task<IEnumerable<Article>> GetAllArticlesAsync();
    Task<Article?> GetArticleByIdAsync(int id);
    Task<IEnumerable<Article>> GetActiveArticlesAsync();
    Task<IEnumerable<Article>> GetRecentArticlesAsync(int count);
    Task<Article> CreateArticleAsync(Article article);
    Task UpdateArticleAsync(Article article);
    Task DeleteArticleAsync(int id);
    Task<IEnumerable<Comment>> GetArticleCommentsAsync(int articleId);
    Task<Comment> AddCommentAsync(Comment comment);
}

