using System.ComponentModel.DataAnnotations;
using SAJT.Cookbook.Domain.Enums;

namespace SAJT.Cookbook.WebApi.Requests.Ingredients;

public sealed class CreateIngredientRequest
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(150)]
    public string? PluralName { get; set; }

    public MeasurementUnit? DefaultUnit { get; set; }

    public bool IsActive { get; set; } = true;
}
