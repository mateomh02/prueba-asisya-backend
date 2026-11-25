
using Asisya.Application.Common.Dto;
using Asisya.Application.Products.Dto;

namespace Asisya.Application.Products.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductResponse>> GetAllAsync(ProductQuery query, CancellationToken ct = default);
        Task<ProductDetailResponse> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ProductDetailResponse> UpdateAsync(int id, UpsertProductRequest request, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<ProductDetailResponse> CreateAsync(UpsertProductRequest request, CancellationToken ct = default);
    }
}
