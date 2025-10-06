using MediatR;
using SAJT.Cookbook.Application.Ingredients.Queries.GetIngredients;
using SAJT.Cookbook.Domain.Enums;

namespace SAJT.Cookbook.Application.Ingredients.Commands.CreateIngredient;

public sealed record CreateIngredientCommand(
    string Name,
    string? PluralName,
    MeasurementUnit? DefaultUnit,
    bool IsActive) : IRequest<CreateIngredientResult>;
