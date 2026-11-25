using Asisya.Application.Common.Dto;
using Asisya.Application.Products.Dto;
using Asisya.Domain;

namespace Asisya.Application.Products.Interfaces
{
    public interface IProductRepository
    {
        Task<PagedResult<Product>> GetPageAsyn(ProductQuery query, CancellationToken ct = default);
        Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
