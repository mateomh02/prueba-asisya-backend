using Asisya.Application.Auth;
using Asisya.Application.Auth.Interfaces;
using Asisya.Application.Routes;
using Asisya.Application.Routes.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Asisya.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRouteService, RouteService>();
        return services;
    }
}