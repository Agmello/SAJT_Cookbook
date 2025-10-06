using Microsoft.EntityFrameworkCore;
using SAJT.Cookbook.Domain.Entities;

namespace SAJT.Cookbook.Infrastructure.Persistence;

public class CookbookDbContext : DbContext
{
    public CookbookDbContext(DbContextOptions<CookbookDbContext> options)
        : base(options)
    {
    }

    public DbSet<Recipe> Recipes => Set<Recipe>();

    public DbSet<RecipeStep> RecipeSteps => Set<RecipeStep>();

    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

    public DbSet<Ingredient> Ingredients => Set<Ingredient>();

    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<User> Users => Set<User>();

    public DbSet<RecipeTag> RecipeTags => Set<RecipeTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CookbookDbContext).Assembly);
    }
}
