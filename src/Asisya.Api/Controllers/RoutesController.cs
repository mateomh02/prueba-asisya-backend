using Asisya.Application.Routes.Dto;
using Asisya.Application.Routes.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asisya.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/routes")]
public class RoutesController : ControllerBase
{
    private readonly IRouteService _routes;

    public RoutesController(IRouteService routes)
    {
        _routes = routes;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RouteResponse>>> GetMine(CancellationToken ct)
    {
        var role = User.FindFirst("role")?.Value;

        if (string.IsNullOrEmpty(role))
            return Forbid();

        return Ok(await _routes.GetForRoleAsync(role, ct));
    }
}