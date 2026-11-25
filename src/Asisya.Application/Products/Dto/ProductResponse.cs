namespace Asisya.Application.Products.Dto
{
    public record ProductResponse(
        int Id,
        string ProductName,
        decimal UnitPrice,
        int UnitsInStock,
        bool Discontinued,
        int CategoryId,
        string CategoryName
        );
}
