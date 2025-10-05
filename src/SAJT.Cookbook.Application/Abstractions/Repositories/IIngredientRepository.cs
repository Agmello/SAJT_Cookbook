using System.Threading;
using System.Threading.Tasks;
using SAJT.Cookbook.Domain.Entities;

namespace SAJT.Cookbook.Application.Abstractions.Repositories;

public interface IIngredientRepository
{
    Task<Ingredient?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<bool> IsNameTakenAsync(string name, long excludeId, CancellationToken cancellationToken = default);

    void Update(Ingredient ingredient);
}
