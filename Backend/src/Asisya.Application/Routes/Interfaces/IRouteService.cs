using Asisya.Application.Routes.Dto;

namespace Asisya.Application.Routes.Interfaces;

public interface IRouteService
{
    Task<IReadOnlyList<RouteResponse>> GetForRoleAsync(string roleName, CancellationToken ct = default);
}