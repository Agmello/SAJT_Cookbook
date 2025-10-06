using SAJT.Cookbook.Domain.Enums;

namespace SAJT.Cookbook.Application.Ingredients.Queries.GetIngredients;

public sealed record IngredientSummaryDto(
    long Id,
    string Name,
    string? PluralName,
    MeasurementUnit? DefaultUnit,
    bool IsActive);
