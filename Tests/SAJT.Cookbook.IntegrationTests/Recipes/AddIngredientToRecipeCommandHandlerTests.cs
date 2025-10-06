using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAJT.Cookbook.Application.Recipes.Commands.AddIngredientToRecipe;
using SAJT.Cookbook.Domain.Entities;
using SAJT.Cookbook.Domain.Enums;
using SAJT.Cookbook.Infrastructure.Persistence;
using SAJT.Cookbook.Infrastructure.Repositories;
using Xunit;

namespace SAJT.Cookbook.IntegrationTests.Recipes;

public sealed class AddIngredientToRecipeCommandHandlerTests
{
    [Fact]
    public async Task Handle_PersistsIngredientEntry()
    {
        var options = new DbContextOptionsBuilder<CookbookDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new CookbookDbContext(options);

        var recipe = Recipe.Create(Guid.NewGuid(), "chili", "Chili", null, 10, 30, 4, RecipeDifficulty.Medium);
        typeof(Recipe).GetProperty(nameof(Recipe.Id))!.SetValue(recipe, 1L);

        var ingredient = Ingredient.Create("Tomato", null, MeasurementUnit.Gram);
        typeof(Ingredient).GetProperty(nameof(Ingredient.Id))!.SetValue(ingredient, 2L);

        await context.Recipes.AddAsync(recipe);
        await context.Ingredients.AddAsync(ingredient);
        await context.SaveChangesAsync();

        var recipeRepository = new RecipeRepository(context);
        var ingredientRepository = new IngredientRepository(context);
        var unitOfWork = new UnitOfWork(context);

        var handler = new AddIngredientToRecipeCommandHandler(recipeRepository, ingredientRepository, unitOfWork);

        var command = new AddIngredientToRecipeCommand(recipe.Id, ingredient.Id, 200, MeasurementUnit.Gram, "Diced");

        var result = await handler.Handle(command, default);

        Assert.Equal(AddIngredientToRecipeStatus.Success, result.Status);
        Assert.NotNull(result.Ingredient);

        var entries = await context.RecipeIngredients.AsNoTracking().ToListAsync();
        Assert.Single(entries);
        Assert.Equal(ingredient.Id, entries[0].IngredientId);
        Assert.Equal(200m, entries[0].Amount);
        Assert.Equal("Diced", entries[0].Note);
    }
}
