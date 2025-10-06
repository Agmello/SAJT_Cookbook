using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAJT.Cookbook.Application.Users.Commands.CreateUser;
using SAJT.Cookbook.Application.Users.Models;
using SAJT.Cookbook.Application.Users.Queries.GetUsers;
using SAJT.Cookbook.WebApi.Requests.Users;

namespace SAJT.Cookbook.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserSummaryDto>>> GetAsync(CancellationToken cancellationToken)
    {
        var users = await _mediator.Send(new GetUsersQuery(), cancellationToken);
        return Ok(users);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserSummaryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var command = new CreateUserCommand(request.Name);
        var result = await _mediator.Send(command, cancellationToken);

        return result.Status switch
        {
            CreateUserStatus.Success => Created($"/api/users/{result.User!.Id}", result.User),
            CreateUserStatus.InvalidName => BadRequest("Name is required and must be at most 200 characters."),
            CreateUserStatus.NameAlreadyExists => Conflict("A user with that name already exists."),
            _ => Problem()
        };
    }
}
