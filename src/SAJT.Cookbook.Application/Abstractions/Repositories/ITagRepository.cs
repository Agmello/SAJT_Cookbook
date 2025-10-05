using System.Threading;
using System.Threading.Tasks;
using SAJT.Cookbook.Domain.Entities;

namespace SAJT.Cookbook.Application.Abstractions.Repositories;

public interface ITagRepository
{
    Task<Tag?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
}
