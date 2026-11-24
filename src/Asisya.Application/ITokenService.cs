using Asisya.Domain.Identity;

namespace Asisya.Application
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
