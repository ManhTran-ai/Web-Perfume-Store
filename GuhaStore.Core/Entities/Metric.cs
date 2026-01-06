namespace GuhaStore.Core.Entities;

public class Metric
{
    public int MetricId { get; set; }
    public DateTime MetricDate { get; set; }
    public int MetricOrder { get; set; }
    public string MetricSales { get; set; } = string.Empty;
    public int MetricQuantity { get; set; }
}

