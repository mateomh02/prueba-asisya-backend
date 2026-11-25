using Asisya.Application.Routes.Interfaces;
using Asisya.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace Asisya.Infrastructure.Persistence.Repositories;

public class RouteRepository : IRouteRepository
{
    private readonly AsisyaDbContext _context;

    public RouteRepository(AsisyaDbContext context)
    {
        _context = context;
    }

    public Task<List<AppRoute>> ListRoutesAsync(string roleName, CancellationToken ct = default)
    {
        return _context.Set<RoleRoute>()
            .AsNoTracking()
            .Where(rr => rr.Role!.RoleName == roleName)
            .Select(rr => rr.AppRoute!)
            .OrderBy(r => r.Id)
            .ToListAsync(ct);
    }
}