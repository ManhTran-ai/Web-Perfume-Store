namespace GuhaStore.Core.Entities;

public class Collection
{
    public int CollectionId { get; set; }
    public string CollectionName { get; set; } = string.Empty;
    public string CollectionKeyword { get; set; } = string.Empty;
    public string CollectionImage { get; set; } = string.Empty;
    public string CollectionDescription { get; set; } = string.Empty;
    public int CollectionOrder { get; set; }
    public int CollectionType { get; set; }
}

