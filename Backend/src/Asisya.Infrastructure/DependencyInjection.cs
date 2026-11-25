using Asisya.Application;
using Asisya.Application.Auth.Interfaces;
using Asisya.Application.Categories.Interfaces;
using Asisya.Application.Products.Interfaces;
using Asisya.Application.Routes.Interfaces;
using Asisya.Infrastructure.Generators;
using Asisya.Infrastructure.Persistence.Bulk;
using Asisya.Infrastructure.Persistence.Repositories;
using Asisya.Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Asisya.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AsisyaDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRouteRepository, RouteRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IRandomProductGenerator, BogusProductGenerator>();
        services.AddScoped<IProductBulkInserter, NpgsqlProductBulkInserter>();
        services.AddScoped<DbInitializer>();

        return services;
    }
}