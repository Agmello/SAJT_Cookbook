using SAJT.Cookbook.Application.Recipes.Models;

namespace SAJT.Cookbook.Application.Recipes.Commands.CreateRecipe;

public enum CreateRecipeStatus
{
    Success,
    InvalidAuthor,
    InvalidTitle,
    InvalidServings,
    InvalidTiming
}

public sealed record CreateRecipeResult(CreateRecipeStatus Status, RecipeSummaryDto? Recipe)
{
    public static CreateRecipeResult Success(RecipeSummaryDto recipe) => new(CreateRecipeStatus.Success, recipe);

    public static CreateRecipeResult InvalidAuthor() => new(CreateRecipeStatus.InvalidAuthor, null);

    public static CreateRecipeResult InvalidTitle() => new(CreateRecipeStatus.InvalidTitle, null);

    public static CreateRecipeResult InvalidServings() => new(CreateRecipeStatus.InvalidServings, null);

    public static CreateRecipeResult InvalidTiming() => new(CreateRecipeStatus.InvalidTiming, null);
}
