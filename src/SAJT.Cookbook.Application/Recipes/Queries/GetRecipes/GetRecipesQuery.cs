using System.Collections.Generic;
using MediatR;
using SAJT.Cookbook.Application.Recipes.Models;

namespace SAJT.Cookbook.Application.Recipes.Queries.GetRecipes;

public sealed record GetRecipesQuery : IRequest<IReadOnlyList<RecipeSummaryDto>>;
