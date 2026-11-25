

namespace Asisya.Application.Products.Dto
{
    public record ProductDetailResponse(
       int Id,
       string ProductName,
       decimal UnitPrice,
       int UnitsInStock,
       bool Discontinued,
       int CategoryId,
       string CategoryName,
       string CategoryPicture,
       string Source,
       int? SupplierId,
       string? QuantityPerUnit,
       short UnitsOnOrder,
       short ReorderLevel
       );
}
