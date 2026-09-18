using Microsoft.AspNetCore.Mvc;
using Modules.Auth.Ports;
using Modules.Auth.Domain;
using Modules.Users.Ports;
using Modules.Users.Domain;
using API.Requests;
using SharedKernel.Interfaces;

namespace Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRegistrationFacade userRegistrationFacade;
    private readonly IAccountRegistrationFacade accountRegistrationFacade;
    private readonly IPasswordHasher passwordHasher;
    private readonly IUnitOfWork unitOfWork;

    public AuthController(
        IUserRegistrationFacade userRegistrationFacade,
        IAccountRegistrationFacade accountRegistrationFacade,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        this.userRegistrationFacade = userRegistrationFacade;
        this.accountRegistrationFacade = accountRegistrationFacade;
        this.passwordHasher = passwordHasher;
        this.unitOfWork = unitOfWork;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AccountRegisterRequest request, CancellationToken cancellationToken)
    {
        var hashedPassword = await passwordHasher.HashPassword(request.Password);
        var user = new User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
        };


        await userRegistrationFacade.HandleAsync(user, cancellationToken);
        return Ok(new { message = "Registration successful" });
    }
}