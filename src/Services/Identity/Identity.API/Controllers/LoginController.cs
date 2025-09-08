using FluentValidation;
using Identity.API.Contracts.Users;
using Identity.Application.Users.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;

[ApiController]
[Route("login")]
public class LoginController(IConfiguration config, LoginUser.IHandler loginUser, IValidator<LoginUser.Command> validatorLogin) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var cmd = new LoginUser.Command(request.Email, request.Password);
        var validation = await validatorLogin.ValidateAsync(cmd, ct);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());
        
        var token = await loginUser.Handle(cmd, ct);
        if (token == null)
            return Unauthorized("Invalid credentials.");
        
        return Ok(new LoginResponse(token));
    }
}