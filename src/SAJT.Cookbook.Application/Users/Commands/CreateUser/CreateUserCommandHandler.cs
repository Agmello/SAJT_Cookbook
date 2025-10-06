using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SAJT.Cookbook.Application.Abstractions.Data;
using SAJT.Cookbook.Application.Abstractions.Repositories;
using SAJT.Cookbook.Application.Users.Models;
using SAJT.Cookbook.Domain.Entities;

namespace SAJT.Cookbook.Application.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return CreateUserResult.InvalidName();
        }

        var trimmedName = request.Name.Trim();
        if (trimmedName.Length > 200)
        {
            return CreateUserResult.InvalidName();
        }

        var isTaken = await _userRepository.IsNameTakenAsync(trimmedName, cancellationToken);
        if (isTaken)
        {
            return CreateUserResult.NameAlreadyExists();
        }

        var user = User.Create(trimmedName);

        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new UserSummaryDto(user.Id, user.Name, user.CreatedAtUtc, user.UpdatedAtUtc);
        return CreateUserResult.Success(dto);
    }
}
