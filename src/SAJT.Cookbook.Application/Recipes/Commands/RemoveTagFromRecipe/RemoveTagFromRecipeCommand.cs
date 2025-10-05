using MediatR;

namespace SAJT.Cookbook.Application.Recipes.Commands.RemoveTagFromRecipe;

public sealed record RemoveTagFromRecipeCommand(long RecipeId, long TagId) : IRequest<RemoveTagFromRecipeResult>;
