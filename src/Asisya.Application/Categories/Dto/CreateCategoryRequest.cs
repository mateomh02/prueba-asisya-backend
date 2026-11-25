using System.ComponentModel.DataAnnotations;

namespace Asisya.Application.Categories.Dto;

public class CreateCategoryRequest
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? Picture { get; set; }
}