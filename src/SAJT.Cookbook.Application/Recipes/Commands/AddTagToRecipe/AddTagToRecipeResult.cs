namespace SAJT.Cookbook.Application.Recipes.Commands.AddTagToRecipe;

public enum AddTagToRecipeStatus
{
    Success,
    RecipeNotFound,
    TagNotFound,
    TagAlreadyAssigned
}

public sealed record AddTagToRecipeResult(AddTagToRecipeStatus Status)
{
    public static AddTagToRecipeResult Success() => new(AddTagToRecipeStatus.Success);

    public static AddTagToRecipeResult RecipeNotFound() => new(AddTagToRecipeStatus.RecipeNotFound);

    public static AddTagToRecipeResult TagNotFound() => new(AddTagToRecipeStatus.TagNotFound);

    public static AddTagToRecipeResult TagAlreadyAssigned() => new(AddTagToRecipeStatus.TagAlreadyAssigned);
}
