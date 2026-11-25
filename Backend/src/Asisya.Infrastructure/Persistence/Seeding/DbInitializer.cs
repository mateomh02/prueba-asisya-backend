using Asisya.Application;
using Asisya.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Asisya.Infrastructure.Persistence.Seeding;

public class DbInitializer
{
    private const int MaxAttempts = 10;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(3);

    private static readonly string[] RoleNames = { "Admin", "User" };

    private static readonly (string Name, string Path, string Component)[] Routes =
    {
        ("products-view", "/products", "products"),
        ("products-edit", "/products/edit", "products"),
        ("category-view", "/category", "category"),
        ("category-edit", "/category/edit", "category")
    };

    private static readonly Dictionary<string, string[]> RoutesByRole = new()
    {
        ["Admin"] = new[] { "products-view", "products-edit", "category-view", "category-edit" },
        ["User"] = new[] { "products-view", "category-view" }
    };

    private readonly AsisyaDbContext _context;
    private readonly IPasswordHasher _hasher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(
        AsisyaDbContext context,
        IPasswordHasher hasher,
        IConfiguration configuration,
        ILogger<DbInitializer> logger)
    {
        _context = context;
        _hasher = hasher;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        await MigrateWithRetryAsync(ct);
        await SeedRolesAsync(ct);
        await SeedRoutesAsync(ct);
        await SeedRoleRoutesAsync(ct);
        await SeedUsersAsync(ct);
        _logger.LogInformation("Base de datos inicializada.");
    }

    private async Task MigrateWithRetryAsync(CancellationToken ct)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                await _context.Database.MigrateAsync(ct);
                return;
            }
            catch (Exception ex) when (attempt < MaxAttempts)
            {
                _logger.LogWarning(ex,
                    "Base de datos no disponible (intento {Attempt}/{Max}). Reintentando en {Seconds}s...",
                    attempt, MaxAttempts, RetryDelay.TotalSeconds);
                await Task.Delay(RetryDelay, ct);
            }
        }
    }

    private async Task SeedRolesAsync(CancellationToken ct)
    {
        foreach (var name in RoleNames)
        {
            if (!await _context.Set<Role>().AnyAsync(r => r.RoleName == name, ct))
                _context.Set<Role>().Add(new Role { RoleName = name });
        }

        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedRoutesAsync(CancellationToken ct)
    {
        foreach (var (name, path, component) in Routes)
        {
            if (!await _context.Set<AppRoute>().AnyAsync(r => r.Name == name, ct))
                _context.Set<AppRoute>().Add(new AppRoute { Name = name, Path = path, Component = component });
        }

        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedRoleRoutesAsync(CancellationToken ct)
    {
        var roles = await _context.Set<Role>().ToDictionaryAsync(r => r.RoleName, r => r.Id, ct);
        var routes = await _context.Set<AppRoute>().ToDictionaryAsync(r => r.Name, r => r.Id, ct);

        foreach (var (roleName, routeNames) in RoutesByRole)
        {
            var roleId = roles[roleName];

            foreach (var routeName in routeNames)
            {
                var routeId = routes[routeName];

                var exists = await _context.Set<RoleRoute>()
                    .AnyAsync(rr => rr.RoleId == roleId && rr.AppRouteId == routeId, ct);

                if (!exists)
                    _context.Set<RoleRoute>().Add(new RoleRoute { RoleId = roleId, AppRouteId = routeId });
            }
        }

        await _context.SaveChangesAsync(ct);
    }

    private async Task SeedUsersAsync(CancellationToken ct)
    {
        var roles = await _context.Set<Role>().ToDictionaryAsync(r => r.RoleName, r => r.Id, ct);

        var users = new[]
        {
            (Role: "Admin", Email: _configuration["Seed:AdminEmail"], Password: _configuration["Seed:AdminPassword"]),
            (Role: "User", Email: _configuration["Seed:UserEmail"], Password: _configuration["Seed:UserPassword"])
        };

        foreach (var (role, email, password) in users)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning(
                    "No se sembró el usuario con rol {Role}: faltan el correo o la contraseña en la configuración (Seed).",
                    role);
                continue;
            }

            if (await _context.Set<User>().AnyAsync(u => u.Email == email, ct))
                continue;

            _context.Set<User>().Add(new User
            {
                Email = email,
                PasswordHash = _hasher.HashPassword(password),
                RoleId = roles[role]
            });
        }

        await _context.SaveChangesAsync(ct);
    }
}
