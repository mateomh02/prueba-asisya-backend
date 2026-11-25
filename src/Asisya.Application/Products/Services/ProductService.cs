using Asisya.Application.Common.Dto;
using Asisya.Application.Common.Exceptions;
using Asisya.Application.Products.Dto;
using Asisya.Application.Products.Interfaces;
using Asisya.Domain;

namespace Asisya.Application.Products.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private const int DefaultPageSize = 10;
        private const int MaxPageSize = 100;
        private static ProductResponse ToResponse(Product p) =>
            new(p.Id, p.ProductName, p.UnitPrice, p.UnitsInStock,
                p.Discontinued, p.CategoryId, p.Category?.Name ?? string.Empty);

        private static ProductDetailResponse ToDetailResponse(Product p) =>
            new(p.Id, p.ProductName, p.UnitPrice, p.UnitsInStock, p.Discontinued, p.CategoryId, p.Category?.Name ?? string.Empty, p.Category?.Picture ?? string.Empty);

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PagedResult<ProductResponse>> GetAllAsync(ProductQuery query, CancellationToken ct = default) 
        {

            var page = Math.Max(query.page, 1);
            var pageSize = query.pageSize < 1
                ? DefaultPageSize
                : Math.Min(query.pageSize, MaxPageSize);

            var normalized = new ProductQuery
            {
                page = page,
                pageSize = pageSize,
                search = query.search,
                categoryId = query.categoryId,
                MinPrice = query.MinPrice,
                MaxPrice = query.MaxPrice
            };


            var result = await _productRepository.GetPageAsyn(normalized, ct);
            var items = result.Items.Select(ToResponse).ToList();
            return new PagedResult<ProductResponse>(items, result.Total, page, pageSize);
        }

        public async Task<ProductDetailResponse> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var product = await _productRepository.GetByIdAsync(id, ct);
            if(product is null)
                throw new NotFoundException("Producto no encontrado");

            return ToDetailResponse(product);
        }
    }
}
