using Microsoft.Extensions.DependencyInjection;

namespace Asisya.Infrastructure.Persistence.Seeding;

public static class DatabaseInitializerExtensions
{
    public static async Task InitializeDatabaseAsync(
        this IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
        await initializer.InitializeAsync(ct);
    }
}
