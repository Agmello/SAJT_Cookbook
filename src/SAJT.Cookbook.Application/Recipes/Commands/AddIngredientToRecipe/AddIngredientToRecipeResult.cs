using SAJT.Cookbook.Application.Recipes.Models;

namespace SAJT.Cookbook.Application.Recipes.Commands.AddIngredientToRecipe;

public enum AddIngredientToRecipeStatus
{
    Success,
    RecipeNotFound,
    IngredientNotFound,
    IngredientAlreadyAssigned
}

public sealed record AddIngredientToRecipeResult(AddIngredientToRecipeStatus Status, RecipeIngredientDto? Ingredient)
{
    public static AddIngredientToRecipeResult Success(RecipeIngredientDto ingredient) => new(AddIngredientToRecipeStatus.Success, ingredient);

    public static AddIngredientToRecipeResult RecipeNotFound() => new(AddIngredientToRecipeStatus.RecipeNotFound, null);

    public static AddIngredientToRecipeResult IngredientNotFound() => new(AddIngredientToRecipeStatus.IngredientNotFound, null);

    public static AddIngredientToRecipeResult IngredientAlreadyAssigned() => new(AddIngredientToRecipeStatus.IngredientAlreadyAssigned, null);
}
