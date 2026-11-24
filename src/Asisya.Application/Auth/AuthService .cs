using Asisya.Application.Auth.Dto;
using Asisya.Application.Auth.Interfaces;

namespace Asisya.Application.Auth
{
    public class AuthService : IAuthService
    {

        private readonly IUserRepository _users;
        private readonly IPasswordHasher _hasher;
        private readonly ITokenService _tokens;

        public AuthService(IUserRepository users, IPasswordHasher hasher, ITokenService tokens)
        {
            _users = users;
            _hasher = hasher;
            _tokens = tokens;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _users.GetByEmailAsync(email, ct);

            if (user is null || !_hasher.VerifyPassword(request.Password, user.PasswordHash))
                return null;

            var token = _tokens.GenerateToken(user);
            return new LoginResponse(token, email, user.RoleId, user.Role.RoleName);
        }

    }
}
