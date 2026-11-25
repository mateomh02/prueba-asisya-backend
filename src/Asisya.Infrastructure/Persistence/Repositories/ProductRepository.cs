using Asisya.Application.Common.Dto;
using Asisya.Application.Products.Dto;
using Asisya.Application.Products.Interfaces;
using Asisya.Domain;
using Microsoft.EntityFrameworkCore;

namespace Asisya.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AsisyaDbContext _context;

        public ProductRepository(AsisyaDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
        {
           return await _context.Set<Product>()
                .AsNoTracking()
                .Include(i => i.Category)
                .FirstOrDefaultAsync(f => f.Id == id, ct);
        }

        public async Task<PagedResult<Product>> GetPageAsyn(ProductQuery filter, CancellationToken ct = default)
        {
            IQueryable<Product> products = _context.Set<Product>()
                .AsNoTracking()
                .Include(p => p.Category);

            if (!string.IsNullOrWhiteSpace(filter.search))
                products = products.Where(p => EF.Functions.ILike(p.ProductName, $"%{filter.search}%"));

            if (filter.categoryId.HasValue)
                products = products.Where(p => p.CategoryId == filter.categoryId.Value);

            if (filter.MinPrice.HasValue)
                products = products.Where(p => p.UnitPrice >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                products = products.Where(p => p.UnitPrice <= filter.MaxPrice.Value);

            var total = await products.CountAsync(ct);

            var items = await products
            .OrderBy(p => p.Id)
            .Skip((filter.page - 1) * filter.pageSize)
            .Take(filter.pageSize)
            .ToListAsync(ct);

            return new PagedResult<Product>(items, total, filter.page, filter.pageSize);
        }
            
    }
}
