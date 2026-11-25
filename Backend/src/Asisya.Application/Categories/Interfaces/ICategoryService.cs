using Asisya.Application.Categories.Dto;

namespace Asisya.Application.Categories.Interfaces;

public interface ICategoryService
{
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken ct = default);
}