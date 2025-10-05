using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAJT.Cookbook.Application.Abstractions.Repositories;
using SAJT.Cookbook.Domain.Entities;
using SAJT.Cookbook.Infrastructure.Persistence;

namespace SAJT.Cookbook.Infrastructure.Repositories;

public class IngredientRepository : IIngredientRepository
{
    private readonly CookbookDbContext _dbContext;

    public IngredientRepository(CookbookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Ingredient?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Ingredients.FirstOrDefaultAsync(ingredient => ingredient.Id == id, cancellationToken);
    }

    public async Task<bool> IsNameTakenAsync(string name, long excludeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Ingredients
            .AnyAsync(ingredient => ingredient.Id != excludeId && ingredient.Name == name, cancellationToken);
    }

    public void Update(Ingredient ingredient)
    {
        _dbContext.Ingredients.Update(ingredient);
    }
}
