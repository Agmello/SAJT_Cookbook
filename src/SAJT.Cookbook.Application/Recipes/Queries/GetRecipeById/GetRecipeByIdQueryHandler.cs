using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SAJT.Cookbook.Application.Abstractions.Repositories;
using SAJT.Cookbook.Application.Recipes.Models;

namespace SAJT.Cookbook.Application.Recipes.Queries.GetRecipeById;

public sealed class GetRecipeByIdQueryHandler : IRequestHandler<GetRecipeByIdQuery, RecipeDetailsDto?>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserRepository _userRepository;

    public GetRecipeByIdQueryHandler(IRecipeRepository recipeRepository, IUserRepository userRepository)
    {
        _recipeRepository = recipeRepository;
        _userRepository = userRepository;
    }

    public async Task<RecipeDetailsDto?> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        var recipe = await _recipeRepository.GetByIdAsync(request.RecipeId, cancellationToken);

        if (recipe is null)
        {
            return null;
        }

        var author = await _userRepository.GetByIdAsync(recipe.AuthorId, cancellationToken);

        var ingredients = recipe.Ingredients
            .OrderBy(entry => entry.Ingredient?.Name)
            .Select(entry => new RecipeIngredientDto(
                entry.Id,
                entry.IngredientId,
                entry.Ingredient?.Name ?? string.Empty,
                entry.Amount,
                entry.Unit,
                entry.Note))
            .ToList();

        var steps = recipe.Steps
            .OrderBy(step => step.StepNumber)
            .Select(step => new RecipeStepDto(
                step.Id,
                step.StepNumber,
                step.Instruction,
                step.DurationMinutes,
                step.MediaUrl))
            .ToList();

        var tags = recipe.Tags
            .OrderBy(tag => tag.Tag.Name)
            .Select(tag => new RecipeTagDto(
                tag.TagId,
                tag.Tag.Name,
                tag.Tag.Slug))
            .ToList();

        return new RecipeDetailsDto(
            recipe.Id,
            recipe.AuthorId,
            author?.Name,
            recipe.Slug,
            recipe.Title,
            recipe.Description,
            recipe.PrepTimeMinutes,
            recipe.CookTimeMinutes,
            recipe.Servings,
            recipe.Difficulty,
            recipe.IsPublished,
            recipe.CreatedAtUtc,
            recipe.UpdatedAtUtc,
            ingredients,
            steps,
            tags);
    }
}
