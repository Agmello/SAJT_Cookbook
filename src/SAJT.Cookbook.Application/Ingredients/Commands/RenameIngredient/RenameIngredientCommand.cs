using MediatR;

namespace SAJT.Cookbook.Application.Ingredients.Commands.RenameIngredient;

public sealed record RenameIngredientCommand(long IngredientId, string Name, string? PluralName) : IRequest<RenameIngredientResult>;
