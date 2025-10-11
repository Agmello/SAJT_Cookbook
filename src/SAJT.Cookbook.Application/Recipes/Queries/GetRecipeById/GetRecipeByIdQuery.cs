using MediatR;
using SAJT.Cookbook.Application.Recipes.Models;

namespace SAJT.Cookbook.Application.Recipes.Queries.GetRecipeById;

public sealed record GetRecipeByIdQuery(long RecipeId) : IRequest<RecipeDetailsDto?>;
