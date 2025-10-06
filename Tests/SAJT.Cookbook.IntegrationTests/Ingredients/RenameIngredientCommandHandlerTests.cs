using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAJT.Cookbook.Application.Ingredients.Commands.RenameIngredient;
using SAJT.Cookbook.Domain.Entities;
using SAJT.Cookbook.Infrastructure.Persistence;
using SAJT.Cookbook.Infrastructure.Repositories;
using Xunit;

namespace SAJT.Cookbook.IntegrationTests.Ingredients;

public sealed class RenameIngredientCommandHandlerTests
{
    [Fact]
    public async Task RenameIngredient_PersistsNewName()
    {
        var options = new DbContextOptionsBuilder<CookbookDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new CookbookDbContext(options);

        var ingredient = Ingredient.Create("Sugar");
        await context.Ingredients.AddAsync(ingredient);
        await context.SaveChangesAsync();

        var handler = CreateHandler(context);

        var result = await handler.Handle(new RenameIngredientCommand(ingredient.Id, "Brown Sugar", "Brown Sugars"), default);

        Assert.Equal(RenameIngredientStatus.Success, result.Status);

        var refreshed = await context.Ingredients.AsNoTracking().SingleAsync(x => x.Id == ingredient.Id);
        Assert.Equal("brown sugar", refreshed.Name);
        Assert.Equal("brown sugars", refreshed.PluralName);
    }

    [Fact]
    public async Task RenameIngredient_ReturnsNameAlreadyExists_WhenAnotherIngredientHasName()
    {
        var options = new DbContextOptionsBuilder<CookbookDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new CookbookDbContext(options);

        var original = Ingredient.Create("Milk");
        var conflicting = Ingredient.Create("Cream");

        await context.Ingredients.AddRangeAsync(original, conflicting);
        await context.SaveChangesAsync();

        var handler = CreateHandler(context);

        var result = await handler.Handle(new RenameIngredientCommand(original.Id, "Cream", null), default);

        Assert.Equal(RenameIngredientStatus.NameAlreadyExists, result.Status);
    }

    private static RenameIngredientCommandHandler CreateHandler(CookbookDbContext context)
    {
        var ingredientRepository = new IngredientRepository(context);
        var unitOfWork = new UnitOfWork(context);
        return new RenameIngredientCommandHandler(ingredientRepository, unitOfWork);
    }
}

