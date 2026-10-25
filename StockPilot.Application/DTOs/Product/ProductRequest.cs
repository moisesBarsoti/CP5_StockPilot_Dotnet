using System.ComponentModel.DataAnnotations;

namespace StockPilot.Application.DTOs.Product;

public class CreateProductRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999999999.99")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    public Guid SupplierId { get; set; }
}