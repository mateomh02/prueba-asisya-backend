using Asisya.Application.Auth;
using Asisya.Application.Auth.Interfaces;
using Asisya.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace Asisya.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AsisyaDbContext _context;

    public UserRepository(AsisyaDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return _context.Set<User>()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }
}