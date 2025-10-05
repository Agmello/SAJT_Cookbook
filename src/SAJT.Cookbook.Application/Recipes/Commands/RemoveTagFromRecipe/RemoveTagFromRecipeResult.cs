namespace SAJT.Cookbook.Application.Recipes.Commands.RemoveTagFromRecipe;

public enum RemoveTagFromRecipeStatus
{
    Success,
    RecipeNotFound,
    TagNotFound,
    TagNotAssigned
}

public sealed record RemoveTagFromRecipeResult(RemoveTagFromRecipeStatus Status)
{
    public static RemoveTagFromRecipeResult Success() => new(RemoveTagFromRecipeStatus.Success);

    public static RemoveTagFromRecipeResult RecipeNotFound() => new(RemoveTagFromRecipeStatus.RecipeNotFound);

    public static RemoveTagFromRecipeResult TagNotFound() => new(RemoveTagFromRecipeStatus.TagNotFound);

    public static RemoveTagFromRecipeResult TagNotAssigned() => new(RemoveTagFromRecipeStatus.TagNotAssigned);
}
