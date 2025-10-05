using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using SAJT.Cookbook.Application.Abstractions.Data;
using SAJT.Cookbook.Application.Abstractions.Repositories;
using SAJT.Cookbook.Application.Recipes.Commands.RemoveTagFromRecipe;
using SAJT.Cookbook.Domain.Entities;
using SAJT.Cookbook.Domain.Enums;
using Xunit;

namespace SAJT.Cookbook.UnitTests.Recipes.Commands;

public sealed class RemoveTagFromRecipeCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock = new();
    private readonly Mock<ITagRepository> _tagRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly RemoveTagFromRecipeCommandHandler _handler;

    public RemoveTagFromRecipeCommandHandlerTests()
    {
        _handler = new RemoveTagFromRecipeCommandHandler(
            _recipeRepositoryMock.Object,
            _tagRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsRecipeNotFound_WhenRecipeMissing()
    {
        _recipeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Recipe?)null);

        var result = await _handler.Handle(new RemoveTagFromRecipeCommand(1, 2), CancellationToken.None);

        Assert.Equal(RemoveTagFromRecipeStatus.RecipeNotFound, result.Status);
    }

    [Fact]
    public async Task Handle_ReturnsTagNotFound_WhenTagMissing()
    {
        var recipe = CreateRecipe();

        _recipeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        _tagRepositoryMock
            .Setup(repo => repo.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tag?)null);

        var result = await _handler.Handle(new RemoveTagFromRecipeCommand(1, 2), CancellationToken.None);

        Assert.Equal(RemoveTagFromRecipeStatus.TagNotFound, result.Status);
    }

    [Fact]
    public async Task Handle_ReturnsTagNotAssigned_WhenRecipeDoesNotContainTag()
    {
        var recipe = CreateRecipe();
        var tag = CreateTag(2, "Vegan");

        _recipeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        _tagRepositoryMock
            .Setup(repo => repo.GetByIdAsync(tag.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        var result = await _handler.Handle(new RemoveTagFromRecipeCommand(1, tag.Id), CancellationToken.None);

        Assert.Equal(RemoveTagFromRecipeStatus.TagNotAssigned, result.Status);
    }

    [Fact]
    public async Task Handle_SuccessfullyRemovesTag()
    {
        var recipe = CreateRecipe();
        var tag = CreateTag(2, "Quick");
        recipe.AddTag(tag);

        _recipeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        _tagRepositoryMock
            .Setup(repo => repo.GetByIdAsync(tag.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new RemoveTagFromRecipeCommand(1, tag.Id), CancellationToken.None);

        Assert.Equal(RemoveTagFromRecipeStatus.Success, result.Status);
        Assert.Empty(recipe.Tags);

        _recipeRepositoryMock.Verify(repo => repo.Update(recipe), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Recipe CreateRecipe()
    {
        var recipe = Recipe.Create(
            authorId: Guid.NewGuid(),
            slug: "test-recipe",
            title: "Test Recipe",
            description: null,
            prepTimeMinutes: 10,
            cookTimeMinutes: 20,
            servings: 4,
            difficulty: RecipeDifficulty.Medium);

        typeof(Recipe)
            .GetProperty(nameof(Recipe.Id))!
            .SetValue(recipe, 1L);

        return recipe;
    }

    private static Tag CreateTag(long id, string name)
    {
        var tag = Tag.Create(name, name.ToLowerInvariant());
        typeof(Tag)
            .GetProperty(nameof(Tag.Id))!
            .SetValue(tag, id);
        return tag;
    }
}
