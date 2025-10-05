using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAJT.Cookbook.Application.Recipes.Commands.AddTagToRecipe;
using SAJT.Cookbook.Application.Recipes.Commands.RemoveTagFromRecipe;
using SAJT.Cookbook.Application.Recipes.Models;
using SAJT.Cookbook.Application.Recipes.Queries.GetRecipes;
using SAJT.Cookbook.WebApi.Requests.Recipes;

namespace SAJT.Cookbook.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RecipesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RecipeSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RecipeSummaryDto>>> GetAsync(CancellationToken cancellationToken)
    {
        var recipes = await _mediator.Send(new GetRecipesQuery(), cancellationToken);

        return Ok(recipes);
    }

    [HttpPost("{recipeId:long}/tags")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddTagAsync(long recipeId, [FromBody] AddRecipeTagRequest request, CancellationToken cancellationToken)
    {
        if (request is null || request.TagId <= 0)
        {
            return BadRequest();
        }

        var result = await _mediator.Send(new AddTagToRecipeCommand(recipeId, request.TagId), cancellationToken);

        return result.Status switch
        {
            AddTagToRecipeStatus.Success => NoContent(),
            AddTagToRecipeStatus.RecipeNotFound => NotFound(),
            AddTagToRecipeStatus.TagNotFound => NotFound(),
            AddTagToRecipeStatus.TagAlreadyAssigned => Conflict(),
            _ => Problem()
        };
    }

    [HttpDelete("{recipeId:long}/tags/{tagId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTagAsync(long recipeId, long tagId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RemoveTagFromRecipeCommand(recipeId, tagId), cancellationToken);

        return result.Status switch
        {
            RemoveTagFromRecipeStatus.Success => NoContent(),
            RemoveTagFromRecipeStatus.RecipeNotFound => NotFound(),
            RemoveTagFromRecipeStatus.TagNotFound => NotFound(),
            RemoveTagFromRecipeStatus.TagNotAssigned => NotFound(),
            _ => Problem()
        };
    }
}
