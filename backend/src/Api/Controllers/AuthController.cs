using Microsoft.AspNetCore.Mvc;
using Modules.Auth.Ports;
using Modules.Auth.Domain;
using API.Requests;

namespace CampusConnect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRegistrationFacade userRegistrationFacade;
    private readonly IPasswordHasher passwordHasher;

    public AuthController(IUserRegistrationFacade userRegistrationFacade, IPasswordHasher passwordHasher)
    {
        this.userRegistrationFacade = userRegistrationFacade;
        this.passwordHasher = passwordHasher;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AccountRegisterRequest request, CancellationToken cancellationToken)
    {
        var hashedPassword = await passwordHasher.HashPassword(request.Password);
        var account = new Account
        {
            Email = request.Email,
            Username = request.Username,
            PasswordHash = hashedPassword
        };

        await userRegistrationFacade.HandleAsync(account, cancellationToken);

        return Ok(new { message = "Registration successful" });
    }
}