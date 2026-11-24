using Asisya.Application.Auth;
using Asisya.Application.Auth.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Asisya.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}