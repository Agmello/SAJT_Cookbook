using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAJT.Cookbook.Application.Abstractions.Repositories;
using SAJT.Cookbook.Domain.Entities;
using SAJT.Cookbook.Infrastructure.Persistence;

namespace SAJT.Cookbook.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly CookbookDbContext _dbContext;

    public TagRepository(CookbookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Tag?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tags.FirstOrDefaultAsync(tag => tag.Id == id, cancellationToken);
    }
}
