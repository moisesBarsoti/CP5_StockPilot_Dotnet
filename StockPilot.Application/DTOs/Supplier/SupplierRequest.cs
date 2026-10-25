using System.ComponentModel.DataAnnotations;

namespace StockPilot.Application.DTOs.Supplier;

public class SupplierRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{8}$")]
    public string ZipCode { get; set; } = string.Empty;
}