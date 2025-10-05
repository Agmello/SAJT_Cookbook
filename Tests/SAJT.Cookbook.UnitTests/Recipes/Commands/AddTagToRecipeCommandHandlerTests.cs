using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using SAJT.Cookbook.Application.Abstractions.Data;
using SAJT.Cookbook.Application.Abstractions.Repositories;
using SAJT.Cookbook.Application.Recipes.Commands.AddTagToRecipe;
using SAJT.Cookbook.Domain.Entities;
using SAJT.Cookbook.Domain.Enums;
using Xunit;

namespace SAJT.Cookbook.UnitTests.Recipes.Commands;

public sealed class AddTagToRecipeCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock = new();
    private readonly Mock<ITagRepository> _tagRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly AddTagToRecipeCommandHandler _handler;

    public AddTagToRecipeCommandHandlerTests()
    {
        _handler = new AddTagToRecipeCommandHandler(
            _recipeRepositoryMock.Object,
            _tagRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsRecipeNotFound_WhenRecipeMissing()
    {
        var command = new AddTagToRecipeCommand(10, 2);

        _recipeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(command.RecipeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Recipe?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(AddTagToRecipeStatus.RecipeNotFound, result.Status);
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

        var result = await _handler.Handle(new AddTagToRecipeCommand(1, 2), CancellationToken.None);

        Assert.Equal(AddTagToRecipeStatus.TagNotFound, result.Status);
    }

    [Fact]
    public async Task Handle_ReturnsTagAlreadyAssigned_WhenRecipeAlreadyHasTag()
    {
        var recipe = CreateRecipe();
        var existingTag = CreateTag(2, "Snack");
        recipe.AddTag(existingTag);

        _recipeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        _tagRepositoryMock
            .Setup(repo => repo.GetByIdAsync(existingTag.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTag);

        var result = await _handler.Handle(new AddTagToRecipeCommand(1, existingTag.Id), CancellationToken.None);

        Assert.Equal(AddTagToRecipeStatus.TagAlreadyAssigned, result.Status);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SuccessfullyAddsTag()
    {
        var recipe = CreateRecipe();
        var tag = CreateTag(3, "Dessert");

        _recipeRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipe);

        _tagRepositoryMock
            .Setup(repo => repo.GetByIdAsync(tag.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new AddTagToRecipeCommand(1, tag.Id), CancellationToken.None);

        Assert.Equal(AddTagToRecipeStatus.Success, result.Status);
        Assert.Single(recipe.Tags, rt => rt.TagId == tag.Id);

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
