using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using SAJT.Cookbook.Application.Abstractions.Data;
using SAJT.Cookbook.Application.Abstractions.Repositories;
using SAJT.Cookbook.Application.Users.Commands.CreateUser;
using SAJT.Cookbook.Domain.Entities;
using Xunit;

namespace SAJT.Cookbook.UnitTests.Users.Commands;

public sealed class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _handler = new CreateUserCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsInvalidName_WhenNameIsEmpty()
    {
        var command = new CreateUserCommand("   ");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(CreateUserStatus.InvalidName, result.Status);
        _userRepositoryMock.Verify(repo => repo.Add(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsInvalidName_WhenNameTooLong()
    {
        var name = new string('a', 201);
        var command = new CreateUserCommand(name);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(CreateUserStatus.InvalidName, result.Status);
        _userRepositoryMock.Verify(repo => repo.Add(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsNameAlreadyExists_WhenDuplicate()
    {
        var command = new CreateUserCommand("Jane Doe");

        _userRepositoryMock
            .Setup(repo => repo.IsNameTakenAsync("Jane Doe", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(CreateUserStatus.NameAlreadyExists, result.Status);
        _userRepositoryMock.Verify(repo => repo.Add(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SuccessfullyCreatesUser()
    {
        var command = new CreateUserCommand("Jane Doe");

        User? addedUser = null;
        _userRepositoryMock
            .Setup(repo => repo.Add(It.IsAny<User>()))
            .Callback<User>(user => addedUser = user);

        _unitOfWorkMock
            .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(CreateUserStatus.Success, result.Status);
        Assert.NotNull(result.User);
        Assert.NotNull(addedUser);
        Assert.Equal("Jane Doe", addedUser!.Name);

        _userRepositoryMock.Verify(repo => repo.Add(It.IsAny<User>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
