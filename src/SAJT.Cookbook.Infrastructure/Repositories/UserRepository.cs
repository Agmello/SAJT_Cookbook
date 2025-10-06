using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAJT.Cookbook.Application.Abstractions.Repositories;
using SAJT.Cookbook.Domain.Entities;
using SAJT.Cookbook.Infrastructure.Persistence;

namespace SAJT.Cookbook.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly CookbookDbContext _dbContext;

    public UserRepository(CookbookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(User user)
    {
        _dbContext.Users.Add(user);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsNameTakenAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalized = name.Trim();
        return await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(user => user.Name == normalized, cancellationToken);
    }
}
