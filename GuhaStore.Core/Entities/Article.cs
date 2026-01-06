namespace GuhaStore.Core.Entities;

public class Article
{
    public int ArticleId { get; set; }
    public string ArticleAuthor { get; set; } = string.Empty;
    public string ArticleTitle { get; set; } = string.Empty;
    public string ArticleSummary { get; set; } = string.Empty;
    public string ArticleContent { get; set; } = string.Empty;
    public string ArticleImage { get; set; } = string.Empty;
    public DateTime ArticleDate { get; set; }
    public int ArticleStatus { get; set; } // 0=Draft, 1=Published
    
    // Navigation properties
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

