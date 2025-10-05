using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAJT.Cookbook.Application.Abstractions.Repositories;
using SAJT.Cookbook.Domain.Entities;
using SAJT.Cookbook.Infrastructure.Persistence;

namespace SAJT.Cookbook.Infrastructure.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly CookbookDbContext _dbContext;

    public RecipeRepository(CookbookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Recipe recipe) => _dbContext.Recipes.Add(recipe);

    public async Task<Recipe?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Recipes
            .Include(recipe => recipe.Steps)
            .Include(recipe => recipe.Ingredients)
                .ThenInclude(entry => entry.Ingredient)
            .Include(recipe => recipe.Tags)
                .ThenInclude(recipeTag => recipeTag.Tag)
            .FirstOrDefaultAsync(recipe => recipe.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Recipe>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Recipes
            .AsNoTracking()
            .OrderBy(recipe => recipe.Title)
            .ToListAsync(cancellationToken);
    }

    public void Remove(Recipe recipe) => _dbContext.Recipes.Remove(recipe);

    public void Update(Recipe recipe) => _dbContext.Recipes.Update(recipe);
}
