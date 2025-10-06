using System.Threading;
using System.Threading.Tasks;
using Moq;
using SAJT.Cookbook.Application.Abstractions.Data;
using SAJT.Cookbook.Application.Abstractions.Repositories;
using SAJT.Cookbook.Application.Ingredients.Commands.RenameIngredient;
using SAJT.Cookbook.Domain.Entities;
using SAJT.Cookbook.Domain.Enums;
using Xunit;

namespace SAJT.Cookbook.UnitTests.Ingredients.Commands;

public sealed class RenameIngredientCommandHandlerTests
{
    private readonly Mock<IIngredientRepository> _ingredientRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly RenameIngredientCommandHandler _handler;

    public RenameIngredientCommandHandlerTests()
    {
        _handler = new RenameIngredientCommandHandler(
            _ingredientRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsInvalidName_WhenNameIsWhitespace()
    {
        var command = new RenameIngredientCommand(1, "   ", null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(RenameIngredientStatus.InvalidName, result.Status);
        _ingredientRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenIngredientDoesNotExist()
    {
        var command = new RenameIngredientCommand(42, "Sugar", null);

        _ingredientRepositoryMock
            .Setup(repo => repo.GetByIdAsync(command.IngredientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ingredient?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(RenameIngredientStatus.NotFound, result.Status);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsNameAlreadyExists_WhenDuplicateDetected()
    {
        const long ingredientId = 5;
        var command = new RenameIngredientCommand(ingredientId, "Flour", null);
        var ingredient = Ingredient.Create("Old Flour");
        SetPrivateId(ingredient, ingredientId);

        _ingredientRepositoryMock
            .Setup(repo => repo.GetByIdAsync(command.IngredientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredient);

        _ingredientRepositoryMock
            .Setup(repo => repo.IsNameTakenAsync(command.Name, ingredientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(RenameIngredientStatus.NameAlreadyExists, result.Status);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SuccessfullyRenamesIngredient()
    {
        const long ingredientId = 7;
        var ingredient = Ingredient.Create("Milk", null, MeasurementUnit.Milliliter);
        SetPrivateId(ingredient, ingredientId);
        var command = new RenameIngredientCommand(ingredientId, "  Whole Milk  ", "Whole Milks");

        _ingredientRepositoryMock
            .Setup(repo => repo.GetByIdAsync(ingredientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredient);

        _ingredientRepositoryMock
            .Setup(repo => repo.IsNameTakenAsync(command.Name, ingredientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(RenameIngredientStatus.Success, result.Status);
        Assert.Equal("whole milk", ingredient.Name);
        Assert.Equal("whole milks", ingredient.PluralName);

        _ingredientRepositoryMock.Verify(repo => repo.Update(ingredient), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static void SetPrivateId(Ingredient ingredient, long id)
    {
        typeof(Ingredient)
            .GetProperty(nameof(Ingredient.Id))!
            .SetValue(ingredient, id);
    }
}

