namespace GuhaStore.Core.Entities;

public class Inventory
{
    public int InventoryId { get; set; }
    public int AccountId { get; set; }
    public string StafName { get; set; } = string.Empty; // Note: typo in DB schema
    public string SupplierName { get; set; } = string.Empty;
    public string SupplierPhone { get; set; } = string.Empty;
    public string InventoryNote { get; set; } = string.Empty;
    public string InventoryCode { get; set; } = string.Empty;
    public DateTime InventoryDate { get; set; }
    public decimal TotalAmount { get; set; }
    public int InventoryStatus { get; set; } // 0=Completed, 1=Pending
    
    // Navigation properties
    public Account? Account { get; set; }
    public ICollection<InventoryDetail> InventoryDetails { get; set; } = new List<InventoryDetail>();
}

