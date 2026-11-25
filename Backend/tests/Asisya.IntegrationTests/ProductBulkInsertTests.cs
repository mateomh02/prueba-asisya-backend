using Asisya.Domain;
using Asisya.Domain.Entities;
using Asisya.Infrastructure;
using Asisya.Infrastructure.Generators;
using Asisya.Infrastructure.Persistence.Bulk;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Asisya.IntegrationTests;

public class ProductBulkInsertTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:17")
        .Build();

    private AsisyaDbContext _context = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var options = new DbContextOptionsBuilder<AsisyaDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        _context = new AsisyaDbContext(options);
        await _context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task BulkInsertAsync_WithRealPostgres_InsertsAllProductsMarkedAsBulkLoad()
    {
        var category = new Category { Name = "SERVIDORES" };
        _context.Set<Category>().Add(category);
        await _context.SaveChangesAsync();

        var products = new BogusProductGenerator().Generate(1000, new[] { category.Id });
        var inserter = new NpgsqlProductBulkInserter(_context);

        var inserted = await inserter.BulkInsertAsync(products);

        Assert.Equal(1000, inserted);
        Assert.Equal(1000, await _context.Set<Product>().CountAsync());
        Assert.Equal(1000, await _context.Set<Product>().CountAsync(p => p.Source == ProductSource.BulkLoad));
    }
}