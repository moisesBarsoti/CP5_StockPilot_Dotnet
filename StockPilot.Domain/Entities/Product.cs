using StockPilot.Domain.Common;

namespace StockPilot.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    public Guid SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
}