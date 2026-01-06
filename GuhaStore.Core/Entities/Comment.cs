namespace GuhaStore.Core.Entities;

public class Comment
{
    public int CommentId { get; set; }
    public int ArticleId { get; set; }
    public string CommentName { get; set; } = string.Empty;
    public string CommentEmail { get; set; } = string.Empty;
    public string CommentContent { get; set; } = string.Empty;
    public DateTime CommentDate { get; set; }
    public int CommentStatus { get; set; } // 0=Pending, 1=Approved
    
    // Navigation properties
    public Article? Article { get; set; }
}

