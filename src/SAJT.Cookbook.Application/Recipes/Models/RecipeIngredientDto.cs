using SAJT.Cookbook.Domain.Enums;

namespace SAJT.Cookbook.Application.Recipes.Models;

public sealed record RecipeIngredientDto(
    long Id,
    long IngredientId,
    string IngredientName,
    decimal Amount,
    MeasurementUnit Unit,
    string? Note);
