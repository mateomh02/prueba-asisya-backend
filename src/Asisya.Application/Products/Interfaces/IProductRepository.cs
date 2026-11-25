using Asisya.Application.Common.Dto;
using Asisya.Application.Products.Dto;
using Asisya.Domain;

namespace Asisya.Application.Products.Interfaces
{
    public interface IProductRepository
    {
        Task<PagedResult<Product>> GetPageAsyn(ProductQuery query, CancellationToken ct = default);
        Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Product?> GetForUpdateAsync(int id, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
        Task DeleteAsync(Product product, CancellationToken ct = default);
        Task<bool> CategoryExistsAsync(int categoryId, CancellationToken ct = default);
        Task<bool> SupplierExistsAsync(int supplierId, CancellationToken ct = default);
        Task AddAsync(Product product, CancellationToken ct = default);
    }
}
