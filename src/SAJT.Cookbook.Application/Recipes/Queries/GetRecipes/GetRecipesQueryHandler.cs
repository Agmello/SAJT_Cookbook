using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SAJT.Cookbook.Application.Abstractions.Repositories;
using SAJT.Cookbook.Application.Recipes.Models;

namespace SAJT.Cookbook.Application.Recipes.Queries.GetRecipes;

public sealed class GetRecipesQueryHandler : IRequestHandler<GetRecipesQuery, IReadOnlyList<RecipeSummaryDto>>
{
    private readonly IRecipeRepository _recipeRepository;

    public GetRecipesQueryHandler(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public async Task<IReadOnlyList<RecipeSummaryDto>> Handle(GetRecipesQuery request, CancellationToken cancellationToken)
    {
        var recipes = await _recipeRepository.ListAsync(cancellationToken);

        return recipes
            .OrderBy(recipe => recipe.Title)
            .Select(recipe => new RecipeSummaryDto(
                recipe.Id,
                recipe.Title,
                recipe.Description,
                recipe.PrepTimeMinutes,
                recipe.CookTimeMinutes,
                recipe.Servings,
                recipe.Difficulty,
                recipe.IsPublished,
                recipe.UpdatedAtUtc))
            .ToList();
    }
}
