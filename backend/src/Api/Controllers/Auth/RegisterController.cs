using Microsoft.AspNetCore.Mvc;
using Modules.Auth.Ports;
using Api.Requests;
using SharedKernel.Interfaces;
using Application.Domain;
using Application.Ports;

namespace Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class RegisterController : ControllerBase
{
    private readonly IRegisterUserUseCase registerUserUseCase;

    public RegisterController(
        IRegisterUserUseCase registerUserUseCase)
    {
        this.registerUserUseCase = registerUserUseCase;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AccountRegisterRequest request, CancellationToken cancellationToken)
    {
        var newAccount = new RegisterAccount
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Username = request.Username,
            Password = request.Password,
        };


        await registerUserUseCase.ExecuteAsync(newAccount, cancellationToken);
        return Ok(new { message = "Registration successful" });
    }
}