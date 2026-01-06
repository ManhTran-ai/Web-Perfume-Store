namespace GuhaStore.Core.Entities;

public class Account
{
    public int AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string AccountPassword { get; set; } = string.Empty;
    public string AccountEmail { get; set; } = string.Empty;
    public string? AccountPhone { get; set; }
    public int AccountType { get; set; } // 0=Customer, 1=Staff, 2=Admin
    public int AccountStatus { get; set; } // 0=Active, 1=Inactive
}

