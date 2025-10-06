using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SAJT.Cookbook.Application.Abstractions.Data;
using SAJT.Cookbook.Application.Abstractions.Repositories;
using SAJT.Cookbook.Application.Recipes.Models;
using SAJT.Cookbook.Domain.Entities;

namespace SAJT.Cookbook.Application.Recipes.Commands.AddIngredientToRecipe;

public sealed class AddIngredientToRecipeCommandHandler : IRequestHandler<AddIngredientToRecipeCommand, AddIngredientToRecipeResult>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddIngredientToRecipeCommandHandler(
        IRecipeRepository recipeRepository,
        IIngredientRepository ingredientRepository,
        IUnitOfWork unitOfWork)
    {
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AddIngredientToRecipeResult> Handle(AddIngredientToRecipeCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.Amount), "Amount must be greater than zero.");
        }

        var recipe = await _recipeRepository.GetByIdAsync(request.RecipeId, cancellationToken);
        if (recipe is null)
        {
            return AddIngredientToRecipeResult.RecipeNotFound();
        }

        var ingredient = await _ingredientRepository.GetByIdAsync(request.IngredientId, cancellationToken);
        if (ingredient is null)
        {
            return AddIngredientToRecipeResult.IngredientNotFound();
        }

        if (recipe.Ingredients.Any(entry => entry.IngredientId == ingredient.Id && entry.Unit == request.Unit))
        {
            return AddIngredientToRecipeResult.IngredientAlreadyAssigned();
        }

        RecipeIngredient entry;
        try
        {
            entry = recipe.AddIngredient(ingredient, request.Amount, request.Unit, request.Note);
        }
        catch (ArgumentException)
        {
            throw;
        }

        _recipeRepository.Update(recipe);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new RecipeIngredientDto(
            entry.Id,
            entry.IngredientId,
            ingredient.Name,
            entry.Amount,
            entry.Unit,
            entry.Note);

        return AddIngredientToRecipeResult.Success(dto);
    }
}
