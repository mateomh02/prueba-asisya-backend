using Asisya.Domain.Identity;
namespace Asisya.Application.Auth.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    }
}
