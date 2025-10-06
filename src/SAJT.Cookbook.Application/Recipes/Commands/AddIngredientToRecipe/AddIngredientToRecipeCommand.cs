using MediatR;
using SAJT.Cookbook.Application.Recipes.Models;
using SAJT.Cookbook.Domain.Enums;

namespace SAJT.Cookbook.Application.Recipes.Commands.AddIngredientToRecipe;

public sealed record AddIngredientToRecipeCommand(
    long RecipeId,
    long IngredientId,
    decimal Amount,
    MeasurementUnit Unit,
    string? Note) : IRequest<AddIngredientToRecipeResult>;
