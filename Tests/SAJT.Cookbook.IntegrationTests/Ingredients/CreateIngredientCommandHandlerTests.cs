using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAJT.Cookbook.Application.Ingredients.Commands.CreateIngredient;
using SAJT.Cookbook.Infrastructure.Persistence;
using SAJT.Cookbook.Infrastructure.Repositories;
using Xunit;

namespace SAJT.Cookbook.IntegrationTests.Ingredients;

public sealed class CreateIngredientCommandHandlerTests
{
    [Fact]
    public async Task Handle_PersistsIngredient()
    {
        var options = new DbContextOptionsBuilder<CookbookDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new CookbookDbContext(options);

        var repository = new IngredientRepository(context);
        var unitOfWork = new UnitOfWork(context);
        var handler = new CreateIngredientCommandHandler(repository, unitOfWork);

        var command = new CreateIngredientCommand("Onion", "Onions", null, true);

        var result = await handler.Handle(command, default);

        Assert.Equal(CreateIngredientStatus.Success, result.Status);
        Assert.NotNull(result.Ingredient);

        var stored = await context.Ingredients.SingleAsync();
        Assert.Equal("Onion", stored.Name);
        Assert.Equal("Onions", stored.PluralName);
        Assert.True(stored.IsActive);
    }
}
