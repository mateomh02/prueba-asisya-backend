using Asisya.Application.Categories.Dto;
using Asisya.Application.Categories.Interfaces;
using Asisya.Application.Common.Exceptions;
using Asisya.Domain.Entities;

namespace Asisya.Application.Categories;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categories;

    public CategoryService(ICategoryRepository categories)
    {
        _categories = categories;
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default)
    {
        var name = request.Name.Trim();

        if (await _categories.ExistsByNameAsync(name, ct))
            throw new ConflictException($"Ya existe una categoría con el nombre '{name}'.");

        var category = new Category
        {
            Name = name,
            Description = request.Description?.Trim(),
            Picture = request.Picture?.Trim()
        };

        await _categories.AddAsync(category, ct);

        return ToResponse(category);
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var categories = await _categories.GetAllAsync(ct);
        return categories.Select(ToResponse).ToList();
    }

    private static CategoryResponse ToResponse(Category c) =>
        new(c.Id, c.Name, c.Description, c.Picture);
}