using System.Threading;
using System.Threading.Tasks;

namespace SAJT.Cookbook.Application.Abstractions.Data;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
