using Asisya.Application.Products.Interfaces;
using Asisya.Domain;
using Bogus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asisya.Infrastructure.Generators
{
    public class BogusProductGenerator : IRandomProductGenerator
    {
        public IEnumerable<Product> Generate(int count, IReadOnlyList<int> categoryIds)
        {
            var ids = categoryIds.ToArray();
            var faker = new Faker<Product>()
                .RuleFor(p => p.ProductName, f => $"{f.Commerce.ProductName()} {f.Random.AlphaNumeric(5).ToUpper()}")
                .RuleFor(p => p.CategoryId, f => ids[f.Random.Int(0, ids.Length - 1)])
                .RuleFor(p => p.SupplierId, (int?)null)
                .RuleFor(p => p.QuantityPerUnit, f => $"{f.Random.Int(1, 50)} unidades")
                .RuleFor(p => p.UnitPrice, f => decimal.Round(f.Random.Decimal(1m, 10_000m), 2))
                .RuleFor(p => p.UnitsInStock, f => (short)f.Random.Int(0, 500))
                .RuleFor(p => p.UnitsOnOrder, f => (short)f.Random.Int(0, 100))
                .RuleFor(p => p.ReorderLevel, f => (short)f.Random.Int(0, 50))
                .RuleFor(p => p.Discontinued, f => f.Random.Bool(0.05f))
                .RuleFor(p => p.Source, ProductSource.BulkLoad);

            return faker.GenerateLazy(count);
        }
    }
}
