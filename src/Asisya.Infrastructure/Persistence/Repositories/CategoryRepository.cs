using Asisya.Application.Categories.Interfaces;
using Asisya.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Asisya.Infrastructure.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AsisyaDbContext _context;

    public CategoryRepository(AsisyaDbContext context)
    {
        _context = context;
    }

    public async Task<Category> AddAsync(Category category, CancellationToken ct = default)
    {
        _context.Set<Category>().Add(category);
        await _context.SaveChangesAsync(ct);
        return category;
    }

    public Task<List<Category>> GetAllAsync(CancellationToken ct = default)
    {
        return _context.Set<Category>()
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(ct);
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
    {
        var normalized = name.ToLower();
        return _context.Set<Category>()
            .AnyAsync(c => c.Name.ToLower() == normalized, ct);
    }

    public Task<List<Category>> GetByNamesAsync(IEnumerable<string> names, CancellationToken ct = default) 
    {
        var normalized = names.Select(n => n.ToLower()).ToList();
        return _context.Set<Category>()
            .AsNoTracking()
            .Where(c => normalized.Contains(c.Name.ToLower()))
            .ToListAsync(ct);
    }
}
