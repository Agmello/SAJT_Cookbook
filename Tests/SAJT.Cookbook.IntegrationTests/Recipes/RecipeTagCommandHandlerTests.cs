using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAJT.Cookbook.Application.Recipes.Commands.AddTagToRecipe;
using SAJT.Cookbook.Application.Recipes.Commands.RemoveTagFromRecipe;
using SAJT.Cookbook.Domain.Entities;
using SAJT.Cookbook.Domain.Enums;
using SAJT.Cookbook.Infrastructure.Persistence;
using SAJT.Cookbook.Infrastructure.Repositories;
using Xunit;

namespace SAJT.Cookbook.IntegrationTests.Recipes;

public sealed class RecipeTagCommandHandlerTests
{
    [Fact]
    public async Task AddTagToRecipe_PersistsRecipeTag()
    {
        var options = new DbContextOptionsBuilder<CookbookDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new CookbookDbContext(options);

        var recipe = Recipe.Create(Guid.NewGuid(), "chili-con-carne", "Chili", null, 15, 45, 4, RecipeDifficulty.Medium);
        var tag = Tag.Create("Dinner", "dinner");

        await context.Recipes.AddAsync(recipe);
        await context.Tags.AddAsync(tag);
        await context.SaveChangesAsync();

        var handler = CreateAddHandler(context);

        var result = await handler.Handle(new AddTagToRecipeCommand(recipe.Id, tag.Id), default);

        Assert.Equal(AddTagToRecipeStatus.Success, result.Status);

        var persisted = await context.RecipeTags.AsNoTracking().SingleAsync(rt => rt.RecipeId == recipe.Id && rt.TagId == tag.Id);
        Assert.NotNull(persisted);
    }

    [Fact]
    public async Task RemoveTagFromRecipe_RemovesRecipeTag()
    {
        var options = new DbContextOptionsBuilder<CookbookDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new CookbookDbContext(options);

        var recipe = Recipe.Create(Guid.NewGuid(), "banana-bread", "Banana Bread", null, 10, 60, 6, RecipeDifficulty.Easy);
        var tag = Tag.Create("Dessert", "dessert");

        await context.Recipes.AddAsync(recipe);
        await context.Tags.AddAsync(tag);
        await context.SaveChangesAsync();

        var addHandler = CreateAddHandler(context);
        var addResult = await addHandler.Handle(new AddTagToRecipeCommand(recipe.Id, tag.Id), default);
        Assert.Equal(AddTagToRecipeStatus.Success, addResult.Status);

        var removeHandler = CreateRemoveHandler(context);
        var removeResult = await removeHandler.Handle(new RemoveTagFromRecipeCommand(recipe.Id, tag.Id), default);

        Assert.Equal(RemoveTagFromRecipeStatus.Success, removeResult.Status);
        Assert.Empty(await context.RecipeTags.AsNoTracking().ToListAsync());
    }

    private static AddTagToRecipeCommandHandler CreateAddHandler(CookbookDbContext context)
    {
        var recipeRepository = new RecipeRepository(context);
        var tagRepository = new TagRepository(context);
        var unitOfWork = new UnitOfWork(context);
        return new AddTagToRecipeCommandHandler(recipeRepository, tagRepository, unitOfWork);
    }

    private static RemoveTagFromRecipeCommandHandler CreateRemoveHandler(CookbookDbContext context)
    {
        var recipeRepository = new RecipeRepository(context);
        var tagRepository = new TagRepository(context);
        var unitOfWork = new UnitOfWork(context);
        return new RemoveTagFromRecipeCommandHandler(recipeRepository, tagRepository, unitOfWork);
    }
}
