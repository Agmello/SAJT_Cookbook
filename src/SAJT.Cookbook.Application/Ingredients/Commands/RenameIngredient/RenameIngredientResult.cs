namespace SAJT.Cookbook.Application.Ingredients.Commands.RenameIngredient;

public enum RenameIngredientStatus
{
    Success,
    NotFound,
    NameAlreadyExists,
    InvalidName
}

public sealed record RenameIngredientResult(RenameIngredientStatus Status)
{
    public static RenameIngredientResult Success() => new(RenameIngredientStatus.Success);

    public static RenameIngredientResult NotFound() => new(RenameIngredientStatus.NotFound);

    public static RenameIngredientResult NameAlreadyExists() => new(RenameIngredientStatus.NameAlreadyExists);

    public static RenameIngredientResult InvalidName() => new(RenameIngredientStatus.InvalidName);
}
