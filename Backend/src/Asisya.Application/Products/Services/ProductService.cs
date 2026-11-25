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
                p.Discontinued, p.CategoryId, p.Category?.Name ?? string.Empty, p.Source.ToString());

        private static ProductDetailResponse ToDetailResponse(Product p) =>
            new(p.Id, p.ProductName, p.UnitPrice, p.UnitsInStock, p.Discontinued, p.CategoryId, p.Category?.Name ?? string.Empty, p.Category?.Picture ?? string.Empty, p.Source.ToString(), p.SupplierId, p.QuantityPerUnit, p.UnitsOnOrder, p.ReorderLevel);

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
            if (product is null)
                throw new NotFoundException("Producto no encontrado");

            return ToDetailResponse(product);
        }

        public async Task<ProductDetailResponse> UpdateAsync(int id, UpsertProductRequest request, CancellationToken ct = default)
        {
            var product = await _productRepository.GetForUpdateAsync(id, ct)
                ?? throw new NotFoundException("Producto no encontrado");

            await EnsureReferencesExistAsync(request, ct);

            ApplyChanges(product, request);
            await _productRepository.SaveChangesAsync(ct);

            return await GetByIdAsync(id, ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var product = await _productRepository.GetForUpdateAsync(id, ct)
                ?? throw new NotFoundException("Producto no encontrado");

            await _productRepository.DeleteAsync(product, ct);
        }

        public async Task<ProductDetailResponse> CreateAsync(UpsertProductRequest request, CancellationToken ct = default)
        {
            await EnsureReferencesExistAsync(request, ct);

            var product = new Product();
            ApplyChanges(product, request);
            product.Source = ProductSource.Manual;
            await _productRepository.AddAsync(product, ct);

            return await GetByIdAsync(product.Id, ct);
        }

        private async Task EnsureReferencesExistAsync(UpsertProductRequest request, CancellationToken ct)
        {
            if (!await _productRepository.CategoryExistsAsync(request.CategoryId, ct))
                throw new NotFoundException("La categoría indicada no existe.");

            if (request.SupplierId.HasValue &&
                !await _productRepository.SupplierExistsAsync(request.SupplierId.Value, ct))
                throw new NotFoundException("El proveedor indicado no existe.");
        }

        private static void ApplyChanges(Product product, UpsertProductRequest request)
        {
            product.ProductName = request.ProductName.Trim();
            product.CategoryId = request.CategoryId;
            product.SupplierId = request.SupplierId;
            product.QuantityPerUnit = request.QuantityPerUnit?.Trim();
            product.UnitPrice = request.UnitPrice;
            product.UnitsInStock = request.UnitsInStock;
            product.UnitsOnOrder = request.UnitsOnOrder;
            product.ReorderLevel = request.ReorderLevel;
            product.Discontinued = request.Discontinued;
        }
    }
}
