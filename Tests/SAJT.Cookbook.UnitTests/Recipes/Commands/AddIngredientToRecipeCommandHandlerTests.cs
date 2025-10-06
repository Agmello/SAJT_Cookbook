using System.Threading;
using System.Threading.Tasks;
using Moq;
using SAJT.Cookbook.Application.Abstractions.Data;
using SAJT.Cookbook.Application.Abstractions.Repositories;
using SAJT.Cookbook.Application.Recipes.Commands.AddIngredientToRecipe;
using SAJT.Cookbook.Domain.Entities;
using SAJT.Cookbook.Domain.Enums;
using Xunit;

namespace SAJT.Cookbook.UnitTests.Recipes.Commands;

public sealed class AddIngredientToRecipeCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock = new();
    private readonly Mock<IIngredientRepository> _ingredientRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly AddIngredientToRecipeCommandHandler _handler;

    public AddIngredientToRecipeCommandHandlerTests()
    {
        _handler = new AddIngredientToRecipeCommandHandler(
            _recipeRepositoryMock.Object,
            _ingredientRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsRecipeNotFound_WhenRecipeMissing()
    {
        var command = new AddIngredientToRecipeCommand(1, 2, 100, MeasurementUnit.Gram, null);

        _recipeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(command.RecipeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Recipe?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(AddIngredientToRecipeStatus.RecipeNotFound, result.Status);
    }

    [Fact]
    public async Task Handle_ReturnsIngredientNotFound_WhenIngredientMissing()
    {
        var recipe = CreateRecipe();

        _recipeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(recipe.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        _ingredientRepositoryMock
            .Setup(repo => repo.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ingredient?)null);

        var command = new AddIngredientToRecipeCommand(recipe.Id, 2, 100, MeasurementUnit.Gram, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(AddIngredientToRecipeStatus.IngredientNotFound, result.Status);
    }

    [Fact]
    public async Task Handle_ReturnsIngredientAlreadyAssigned_WhenDuplicate()
    {
        var recipe = CreateRecipe();
        var ingredient = Ingredient.Create("Sugar");
        typeof(Ingredient).GetProperty(nameof(Ingredient.Id))!.SetValue(ingredient, 2L);

        recipe.AddIngredient(ingredient, 100, MeasurementUnit.Gram, null);

        _recipeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(recipe.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        _ingredientRepositoryMock
            .Setup(repo => repo.GetByIdAsync(ingredient.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredient);

        var command = new AddIngredientToRecipeCommand(recipe.Id, ingredient.Id, 50, MeasurementUnit.Gram, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(AddIngredientToRecipeStatus.IngredientAlreadyAssigned, result.Status);
    }

    [Fact]
    public async Task Handle_SuccessfullyAddsIngredient()
    {
        var recipe = CreateRecipe();
        var ingredient = Ingredient.Create("Flour");
        typeof(Ingredient).GetProperty(nameof(Ingredient.Id))!.SetValue(ingredient, 3L);

        _recipeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(recipe.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        _ingredientRepositoryMock
            .Setup(repo => repo.GetByIdAsync(ingredient.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredient);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new AddIngredientToRecipeCommand(recipe.Id, ingredient.Id, 250, MeasurementUnit.Gram, "Sifted");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(AddIngredientToRecipeStatus.Success, result.Status);
        Assert.NotNull(result.Ingredient);
        Assert.Equal(ingredient.Id, result.Ingredient!.IngredientId);
        Assert.Single(recipe.Ingredients);

        _recipeRepositoryMock.Verify(repo => repo.Update(recipe), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Recipe CreateRecipe()
    {
        var recipe = Recipe.Create(Guid.NewGuid(), "test-recipe", "Test Recipe", null, 10, 20, 4, RecipeDifficulty.Easy);
        typeof(Recipe).GetProperty(nameof(Recipe.Id))!.SetValue(recipe, 1L);
        return recipe;
    }
}

