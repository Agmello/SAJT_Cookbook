using System.ComponentModel.DataAnnotations;
using SAJT.Cookbook.Domain.Enums;

namespace SAJT.Cookbook.WebApi.Requests.Recipes;

public sealed class CreateRecipeRequest
{
    [Required]
    public Guid AuthorId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Range(0, 1440)]
    public int PrepTimeMinutes { get; set; }

    [Range(0, 1440)]
    public int CookTimeMinutes { get; set; }

    [Range(1, 255)]
    public byte Servings { get; set; }

    [Required]
    public RecipeDifficulty Difficulty { get; set; } = RecipeDifficulty.Unknown;

    public bool IsPublished { get; set; }
}
