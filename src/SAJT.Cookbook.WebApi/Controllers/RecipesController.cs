using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAJT.Cookbook.Application.Recipes.Models;
using SAJT.Cookbook.Application.Recipes.Queries.GetRecipes;

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
}
