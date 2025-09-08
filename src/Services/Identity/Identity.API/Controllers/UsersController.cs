using Common.Enum;
using Identity.Application.Users.Commands;
using Identity.API.Contracts.Users;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using MassTransit.Internals;
using Microsoft.AspNetCore.Authorization;

namespace Identity.API.Controllers;

[ApiController]
[Route("users")]
public class UsersController(CreateUser.IHandler createUser, IValidator<CreateUser.Command> validator,
    GetUser.IHandler getUser, IValidator<GetUser.Command> validatorGet) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var cmd = new CreateUser.Command(request.Name, request.Email, request.Password, UserProfile.Seller);
        var validation = await validator.ValidateAsync(cmd, ct);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        var id = await createUser.Handle(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new UserResponse(id, request.Name, request.Email, UserProfile.Seller));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var cmd = new GetUser.Command(id);
        var validation = await validatorGet.ValidateAsync(cmd, ct);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        var user = await getUser.Handle(cmd, ct);
        if (user == null)
            return new NotFoundResult();
        return Ok(new UserResponse(id, user.Name, user.Email, (UserProfile)Enum.Parse(typeof(UserProfile), user.Profile)));
    }
}
