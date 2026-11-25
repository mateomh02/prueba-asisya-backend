using Asisya.Application.Products.Dto;
using Asisya.Domain.Entities;

namespace Asisya.Application.Products.Interfaces
{
    public interface IProductBulkService
    {
        Task<BulkCreateProductsResponse> GenerateRandomAsync(BulkCreateProductsRequest request, CancellationToken ct = default);
    }
}
