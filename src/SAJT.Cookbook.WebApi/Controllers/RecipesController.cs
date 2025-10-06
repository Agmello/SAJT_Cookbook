using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAJT.Cookbook.Application.Recipes.Commands.AddIngredientToRecipe;
using SAJT.Cookbook.Application.Recipes.Commands.AddTagToRecipe;
using SAJT.Cookbook.Application.Recipes.Commands.CreateRecipe;
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

    [HttpPost]
    [ProducesResponseType(typeof(RecipeSummaryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateRecipeRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var command = new CreateRecipeCommand(
            request.AuthorId,
            request.Title,
            request.Description,
            request.PrepTimeMinutes,
            request.CookTimeMinutes,
            request.Servings,
            request.Difficulty,
            request.IsPublished);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Status switch
        {
            CreateRecipeStatus.Success => Created($"/api/recipes/{result.Recipe!.Id}", result.Recipe),
            CreateRecipeStatus.InvalidAuthor => BadRequest("Invalid author identifier."),
            CreateRecipeStatus.InvalidTitle => BadRequest("Title is required."),
            CreateRecipeStatus.InvalidServings => BadRequest("Servings must be at least one."),
            CreateRecipeStatus.InvalidTiming => BadRequest("Preparation and cook times must be non-negative."),
            _ => Problem()
        };
    }

    [HttpPost("{recipeId:long}/ingredients")]
    [ProducesResponseType(typeof(RecipeIngredientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddIngredientAsync(long recipeId, [FromBody] AddRecipeIngredientRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest();
        }

        var command = new AddIngredientToRecipeCommand(recipeId, request.IngredientId, request.Amount, request.Unit, request.Note);
        var result = await _mediator.Send(command, cancellationToken);

        return result.Status switch
        {
            AddIngredientToRecipeStatus.Success => Created($"/api/recipes/{recipeId}/ingredients/{result.Ingredient!.Id}", result.Ingredient),
            AddIngredientToRecipeStatus.RecipeNotFound => NotFound(),
            AddIngredientToRecipeStatus.IngredientNotFound => NotFound(),
            AddIngredientToRecipeStatus.IngredientAlreadyAssigned => Conflict(),
            _ => Problem()
        };
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
