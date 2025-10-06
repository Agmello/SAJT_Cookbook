using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SAJT.Cookbook.Domain.Entities;

namespace SAJT.Cookbook.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default);

    Task<bool> IsNameTakenAsync(string name, CancellationToken cancellationToken = default);

    void Add(User user);
}


