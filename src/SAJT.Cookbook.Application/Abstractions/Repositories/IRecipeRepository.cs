using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SAJT.Cookbook.Domain.Entities;

namespace SAJT.Cookbook.Application.Abstractions.Repositories;

public interface IRecipeRepository
{
    Task<Recipe?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Recipe>> ListAsync(CancellationToken cancellationToken = default);

    void Add(Recipe recipe);

    void Update(Recipe recipe);

    void Remove(Recipe recipe);
}
