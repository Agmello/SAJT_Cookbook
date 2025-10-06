using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAJT.Cookbook.Application.Recipes.Commands.CreateRecipe;
using SAJT.Cookbook.Domain.Enums;
using SAJT.Cookbook.Infrastructure.Persistence;
using SAJT.Cookbook.Infrastructure.Repositories;
using Xunit;

namespace SAJT.Cookbook.IntegrationTests.Recipes;

public sealed class CreateRecipeCommandHandlerTests
{
    [Fact]
    public async Task Handle_PersistsRecipe()
    {
        var options = new DbContextOptionsBuilder<CookbookDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new CookbookDbContext(options);

        var repository = new RecipeRepository(context);
        var unitOfWork = new UnitOfWork(context);
        var handler = new CreateRecipeCommandHandler(repository, unitOfWork);

        var command = new CreateRecipeCommand(
            Guid.NewGuid(),
            "Pasta Primavera",
            "Fresh vegetables with pasta",
            15,
            20,
            3,
            RecipeDifficulty.Easy,
            true);

        var result = await handler.Handle(command, default);

        Assert.Equal(CreateRecipeStatus.Success, result.Status);
        Assert.NotNull(result.Recipe);
        Assert.True(result.Recipe!.Id > 0);

        var stored = await context.Recipes.SingleAsync();
        Assert.Equal("Pasta Primavera", stored.Title);
        Assert.True(stored.IsPublished);
    }
}
