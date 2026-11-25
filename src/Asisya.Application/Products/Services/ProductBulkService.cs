using Asisya.Application.Categories.Interfaces;
using Asisya.Application.Common.Exceptions;
using Asisya.Application.Products.Dto;
using Asisya.Application.Products.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Asisya.Application.Products.Services
{
    public class ProductBulkService : IProductBulkService
    {
        private static readonly string[] CategoryNames = { "SERVIDORES", "CLOUD" };

        private readonly ICategoryRepository _categories;
        private readonly IRandomProductGenerator _generator;
        private readonly IProductBulkInserter _inserter;

        public ProductBulkService(
            ICategoryRepository categories,
            IRandomProductGenerator generator,
            IProductBulkInserter inserter)
        {
            _categories = categories;
            _generator = generator;
            _inserter = inserter;
        }

        public async Task<BulkCreateProductsResponse> GenerateRandomAsync(BulkCreateProductsRequest request, CancellationToken ct = default) 
        {
            var categories = await _categories.GetByNamesAsync(CategoryNames, ct);
            var found = categories.Select(c => c.Name);
            var missing = CategoryNames.Except(found, StringComparer.OrdinalIgnoreCase).ToList();

            if (missing.Count > 0)
                throw new NotFoundException(
                    $"Faltan las categorías: {string.Join(", ", missing)}. Créalas primero con POST /Category.");

            var categoriesIds = categories.Select(c => c.Id).ToList();
            var stopwatch = Stopwatch.StartNew();
            var products = _generator.Generate(request.Count, categoriesIds);
            var inserted = await _inserter.BulkInsertAsync(products, ct);
            stopwatch.Stop();
            return new BulkCreateProductsResponse(inserted, stopwatch.ElapsedMilliseconds);

        }
    }
}
