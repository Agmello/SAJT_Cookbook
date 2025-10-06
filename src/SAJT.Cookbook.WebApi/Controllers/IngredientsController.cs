using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAJT.Cookbook.Application.Ingredients.Commands.CreateIngredient;
using SAJT.Cookbook.Application.Ingredients.Commands.RenameIngredient;
using SAJT.Cookbook.Application.Ingredients.Queries.GetIngredients;
using SAJT.Cookbook.WebApi.Requests.Ingredients;

namespace SAJT.Cookbook.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public IngredientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<IngredientSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<IngredientSummaryDto>>> GetAsync(CancellationToken cancellationToken)
    {
        var ingredients = await _mediator.Send(new GetIngredientsQuery(), cancellationToken);
        return Ok(ingredients);
    }

    [HttpPost]
    [ProducesResponseType(typeof(IngredientSummaryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateIngredientRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var command = new CreateIngredientCommand(request.Name, request.PluralName, request.DefaultUnit, request.IsActive);
        var result = await _mediator.Send(command, cancellationToken);

        return result.Status switch
        {
            CreateIngredientStatus.Success => Created($"/api/ingredients/{result.Ingredient!.Id}", result.Ingredient),
            CreateIngredientStatus.InvalidName => BadRequest("Name is required."),
            CreateIngredientStatus.NameAlreadyExists => Conflict("An ingredient with that name already exists."),
            _ => Problem()
        };
    }
    [HttpPut("{ingredientId:long}/name")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RenameAsync(long ingredientId, [FromBody] RenameIngredientRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest();
        }

        var result = await _mediator.Send(new RenameIngredientCommand(ingredientId, request.Name, request.PluralName), cancellationToken);

        return result.Status switch
        {
            RenameIngredientStatus.Success => NoContent(),
            RenameIngredientStatus.InvalidName => BadRequest(),
            RenameIngredientStatus.NotFound => NotFound(),
            RenameIngredientStatus.NameAlreadyExists => Conflict(),
            _ => Problem()
        };
    }
}

