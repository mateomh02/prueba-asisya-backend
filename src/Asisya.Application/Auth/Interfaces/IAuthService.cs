

using Asisya.Application.Auth.Dto;

namespace Asisya.Application.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default);
    }
}
