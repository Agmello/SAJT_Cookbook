using MediatR;
using SAJT.Cookbook.Domain.Enums;

namespace SAJT.Cookbook.Application.Recipes.Commands.CreateRecipe;

public sealed record CreateRecipeCommand(
    Guid AuthorId,
    string Title,
    string? Description,
    int PrepTimeMinutes,
    int CookTimeMinutes,
    byte Servings,
    RecipeDifficulty Difficulty,
    bool IsPublished) : IRequest<CreateRecipeResult>;
