using System.ComponentModel.DataAnnotations;
using SAJT.Cookbook.Domain.Enums;

namespace SAJT.Cookbook.WebApi.Requests.Recipes;

public sealed class AddRecipeIngredientRequest
{
    [Required]
    public long IngredientId { get; set; }

    [Required]
    [Range(0.01, 100000)]
    public decimal Amount { get; set; }

    [Required]
    public MeasurementUnit Unit { get; set; }

    [StringLength(200)]
    public string? Note { get; set; }
}
