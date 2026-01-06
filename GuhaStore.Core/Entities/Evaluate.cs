namespace GuhaStore.Core.Entities;

public class Evaluate
{
    public int EvaluateId { get; set; }
    public int AccountId { get; set; }
    public int ProductId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public int EvaluateRate { get; set; } // 1-5 stars
    public string EvaluateContent { get; set; } = string.Empty;
    public string EvaluateDate { get; set; } = string.Empty;
    public int EvaluateStatus { get; set; } // 0=Pending, 1=Approved
    
    // Navigation properties
    public Account? Account { get; set; }
    public Product? Product { get; set; }
}

