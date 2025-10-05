using System.Threading;
using System.Threading.Tasks;
using SAJT.Cookbook.Application.Abstractions.Data;

namespace SAJT.Cookbook.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly CookbookDbContext _dbContext;

    public UnitOfWork(CookbookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
