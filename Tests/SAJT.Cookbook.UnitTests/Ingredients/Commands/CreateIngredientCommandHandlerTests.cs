using System.Threading;
using System.Threading.Tasks;
using Moq;
using SAJT.Cookbook.Application.Abstractions.Data;
using SAJT.Cookbook.Application.Abstractions.Repositories;
using SAJT.Cookbook.Application.Ingredients.Commands.CreateIngredient;
using SAJT.Cookbook.Application.Ingredients.Queries.GetIngredients;
using SAJT.Cookbook.Domain.Entities;
using SAJT.Cookbook.Domain.Enums;
using Xunit;

namespace SAJT.Cookbook.UnitTests.Ingredients.Commands;

public sealed class CreateIngredientCommandHandlerTests
{
    private readonly Mock<IIngredientRepository> _ingredientRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly CreateIngredientCommandHandler _handler;

    public CreateIngredientCommandHandlerTests()
    {
        _handler = new CreateIngredientCommandHandler(_ingredientRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsInvalidName_WhenNameIsEmpty()
    {
        var command = new CreateIngredientCommand("  ", null, null, true);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(CreateIngredientStatus.InvalidName, result.Status);
    }

    [Fact]
    public async Task Handle_ReturnsNameAlreadyExists_WhenDuplicate()
    {
        var command = new CreateIngredientCommand("Tomato", null, null, true);

        _ingredientRepositoryMock
            .Setup(repo => repo.IsNameTakenAsync("Tomato", 0, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(CreateIngredientStatus.NameAlreadyExists, result.Status);
    }

    [Fact]
    public async Task Handle_SuccessfullyCreatesIngredient()
    {
        var command = new CreateIngredientCommand("Tomato", "Tomatoes", MeasurementUnit.Piece, false);

        Ingredient? addedIngredient = null;
        _ingredientRepositoryMock
            .Setup(repo => repo.Add(It.IsAny<Ingredient>()))
            .Callback<Ingredient>(ingredient => addedIngredient = ingredient);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(CreateIngredientStatus.Success, result.Status);
        Assert.NotNull(result.Ingredient);
        Assert.NotNull(addedIngredient);
        Assert.Equal("tomato", addedIngredient!.Name);
        Assert.Equal("tomatoes", addedIngredient!.PluralName);
        Assert.Equal("tomato", result.Ingredient!.Name);
        Assert.False(addedIngredient.IsActive);

        _ingredientRepositoryMock.Verify(repo => repo.Add(It.IsAny<Ingredient>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

