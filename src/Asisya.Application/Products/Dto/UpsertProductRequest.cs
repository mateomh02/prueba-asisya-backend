using System.ComponentModel.DataAnnotations;

namespace Asisya.Application.Products.Dto;

public class UpsertProductRequest
{
    [Required, MaxLength(100)]
    public string ProductName { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [Range(1, int.MaxValue)]
    public int? SupplierId { get; set; }

    [MaxLength(50)]
    public string? QuantityPerUnit { get; set; }

    [Range(0.0, 1_000_000_000.0)]
    public decimal UnitPrice { get; set; }

    [Range(0, short.MaxValue)]
    public short UnitsInStock { get; set; }

    [Range(0, short.MaxValue)]
    public short UnitsOnOrder { get; set; }

    [Range(0, short.MaxValue)]
    public short ReorderLevel { get; set; }

    public bool Discontinued { get; set; }
}
