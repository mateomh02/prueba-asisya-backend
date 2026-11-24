using Asisya.Application.Routes.Dto;
using Asisya.Application.Routes.Interfaces;

namespace Asisya.Application.Routes;

public class RouteService : IRouteService
{
    private readonly IRouteRepository _routes;

    public RouteService(IRouteRepository routes)
    {
        _routes = routes;
    }

    public async Task<IReadOnlyList<RouteResponse>> GetForRoleAsync(string roleName, CancellationToken ct = default)
    {
        var routes = await _routes.ListRoutesAsync(roleName, ct);

        return routes
            .Select(r => new RouteResponse(r.Name, r.Path, r.Component))
            .ToList();
    }
}