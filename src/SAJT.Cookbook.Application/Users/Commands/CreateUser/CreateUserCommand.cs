using MediatR;

namespace SAJT.Cookbook.Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand(string Name) : IRequest<CreateUserResult>;
