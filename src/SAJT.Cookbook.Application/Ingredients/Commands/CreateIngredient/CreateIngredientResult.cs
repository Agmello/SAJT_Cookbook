using SAJT.Cookbook.Application.Ingredients.Queries.GetIngredients;

namespace SAJT.Cookbook.Application.Ingredients.Commands.CreateIngredient;

public enum CreateIngredientStatus
{
    Success,
    InvalidName,
    NameAlreadyExists
}

public sealed record CreateIngredientResult(CreateIngredientStatus Status, IngredientSummaryDto? Ingredient)
{
    public static CreateIngredientResult Success(IngredientSummaryDto ingredient) => new(CreateIngredientStatus.Success, ingredient);

    public static CreateIngredientResult InvalidName() => new(CreateIngredientStatus.InvalidName, null);

    public static CreateIngredientResult NameAlreadyExists() => new(CreateIngredientStatus.NameAlreadyExists, null);
}
