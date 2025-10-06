using SAJT.Cookbook.Application.Users.Models;

namespace SAJT.Cookbook.Application.Users.Commands.CreateUser;

public enum CreateUserStatus
{
    Success,
    InvalidName,
    NameAlreadyExists
}

public sealed record CreateUserResult(CreateUserStatus Status, UserSummaryDto? User)
{
    public static CreateUserResult Success(UserSummaryDto user) => new(CreateUserStatus.Success, user);

    public static CreateUserResult InvalidName() => new(CreateUserStatus.InvalidName, null);

    public static CreateUserResult NameAlreadyExists() => new(CreateUserStatus.NameAlreadyExists, null);
}
