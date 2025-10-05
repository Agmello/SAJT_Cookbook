using System.ComponentModel.DataAnnotations;

namespace SAJT.Cookbook.WebApi.Requests.Ingredients;

public sealed class RenameIngredientRequest
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(150)]
    public string? PluralName { get; set; }
}
