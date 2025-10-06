using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using SAJT.Cookbook.Application.Abstractions.Data;
using SAJT.Cookbook.Application.Abstractions.Repositories;
using SAJT.Cookbook.Application.Recipes.Commands.CreateRecipe;
using SAJT.Cookbook.Domain.Entities;
using SAJT.Cookbook.Domain.Enums;
using Xunit;

namespace SAJT.Cookbook.UnitTests.Recipes.Commands;

public sealed class CreateRecipeCommandHandlerTests
{
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly CreateRecipeCommandHandler _handler;

    public CreateRecipeCommandHandlerTests()
    {
        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => CreateUser(id));

        _handler = new CreateRecipeCommandHandler(_recipeRepositoryMock.Object, _userRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsInvalidAuthor_WhenAuthorIdIsEmpty()
    {
        var command = new CreateRecipeCommand(Guid.Empty, "Title", null, 10, 20, 2, RecipeDifficulty.Easy, false);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(CreateRecipeStatus.InvalidAuthor, result.Status);
        _recipeRepositoryMock.Verify(repo => repo.Add(It.IsAny<Recipe>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsInvalidAuthor_WhenUserNotFound()
    {
        var authorId = Guid.NewGuid();
        var command = new CreateRecipeCommand(authorId, "Title", null, 10, 20, 2, RecipeDifficulty.Easy, false);

        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(CreateRecipeStatus.InvalidAuthor, result.Status);
        _recipeRepositoryMock.Verify(repo => repo.Add(It.IsAny<Recipe>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsInvalidTitle_WhenTitleMissing()
    {
        var authorId = Guid.NewGuid();
        var command = new CreateRecipeCommand(authorId, "  ", null, 10, 20, 2, RecipeDifficulty.Easy, false);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(CreateRecipeStatus.InvalidTitle, result.Status);
        _recipeRepositoryMock.Verify(repo => repo.Add(It.IsAny<Recipe>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsInvalidServings_WhenServingsIsZero()
    {
        var authorId = Guid.NewGuid();
        var command = new CreateRecipeCommand(authorId, "Test", null, 10, 20, 0, RecipeDifficulty.Easy, false);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(CreateRecipeStatus.InvalidServings, result.Status);
        _recipeRepositoryMock.Verify(repo => repo.Add(It.IsAny<Recipe>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsInvalidTiming_WhenTimingsNegative()
    {
        var authorId = Guid.NewGuid();
        var command = new CreateRecipeCommand(authorId, "Test", null, -1, 10, 2, RecipeDifficulty.Easy, false);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(CreateRecipeStatus.InvalidTiming, result.Status);
        _recipeRepositoryMock.Verify(repo => repo.Add(It.IsAny<Recipe>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SuccessfullyCreatesRecipe()
    {
        var authorId = Guid.NewGuid();
        var command = new CreateRecipeCommand(authorId, "Chocolate Cake", "Rich and moist", 20, 30, 8, RecipeDifficulty.Medium, true);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        Recipe? addedRecipe = null;
        _recipeRepositoryMock
            .Setup(repo => repo.Add(It.IsAny<Recipe>()))
            .Callback<Recipe>(recipe => addedRecipe = recipe);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(CreateRecipeStatus.Success, result.Status);
        Assert.NotNull(result.Recipe);
        Assert.NotNull(addedRecipe);
        Assert.Equal("Chocolate Cake", addedRecipe!.Title);
        Assert.True(addedRecipe.IsPublished);

        _recipeRepositoryMock.Verify(repo => repo.Add(It.IsAny<Recipe>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static User CreateUser(Guid id)
    {
        var user = User.Create("Test User");
        typeof(User).GetProperty(nameof(User.Id))!.SetValue(user, id);
        return user;
    }
}
