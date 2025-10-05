using MediatR;

namespace SAJT.Cookbook.Application.Recipes.Commands.AddTagToRecipe;

public sealed record AddTagToRecipeCommand(long RecipeId, long TagId) : IRequest<AddTagToRecipeResult>;
