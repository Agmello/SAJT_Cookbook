using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAJT.Cookbook.Application.Recipes.Queries.GetRecipeById;
using SAJT.Cookbook.Domain.Entities;
using SAJT.Cookbook.Domain.Enums;
using SAJT.Cookbook.Infrastructure.Persistence;
using SAJT.Cookbook.Infrastructure.Repositories;
using Xunit;

namespace SAJT.Cookbook.IntegrationTests.Recipes;

public sealed class GetRecipeByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsRecipeDetails_WhenRecipeExists()
    {
        var options = new DbContextOptionsBuilder<CookbookDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new CookbookDbContext(options);

        var author = User.Create("Integration Author");
        var ingredient = Ingredient.Create("flour");
        var tag = Tag.Create("Dessert", "dessert");

        context.Users.Add(author);
        context.Ingredients.Add(ingredient);
        context.Tags.Add(tag);

        var recipe = Recipe.Create(author.Id, "test-slug", "Chocolate Cake", "Rich and moist", 15, 30, 8, RecipeDifficulty.Medium);
        recipe.AddStep(1, "Combine dry ingredients.", 5);
        recipe.AddIngredient(ingredient, 250, MeasurementUnit.Gram, "Sifted");
        recipe.AddTag(tag);

        context.Recipes.Add(recipe);
        await context.SaveChangesAsync();

        var handler = new GetRecipeByIdQueryHandler(new RecipeRepository(context), new UserRepository(context));

        var result = await handler.Handle(new GetRecipeByIdQuery(recipe.Id), default);

        Assert.NotNull(result);
        Assert.Equal(recipe.Id, result!.Id);
        Assert.Equal("test-slug", result.Slug);
        Assert.Equal(author.Id, result.AuthorId);
        Assert.Equal(author.Name, result.AuthorName);
        Assert.Single(result.Ingredients);
        Assert.Single(result.Steps);
        Assert.Single(result.Tags);
        Assert.Equal("flour", result.Ingredients[0].IngredientName);
        Assert.Equal("Combine dry ingredients.", result.Steps[0].Instruction);
        Assert.Equal("dessert", result.Tags[0].Slug);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenRecipeMissing()
    {
        var options = new DbContextOptionsBuilder<CookbookDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new CookbookDbContext(options);

        var handler = new GetRecipeByIdQueryHandler(new RecipeRepository(context), new UserRepository(context));

        var result = await handler.Handle(new GetRecipeByIdQuery(42), default);

        Assert.Null(result);
    }
}
