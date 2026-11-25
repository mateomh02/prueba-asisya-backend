using Asisya.Domain.Entities;

namespace Asisya.Application.Categories.Interfaces;

public interface ICategoryRepository
{
    Task<Category> AddAsync(Category category, CancellationToken ct = default);
    Task<List<Category>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
    Task<List<Category>> GetByNamesAsync(IEnumerable<string> names, CancellationToken ct = default);
}