
using API.Requests;
using Microsoft.AspNetCore.Mvc;
using Modules.Auth.Domain;
using Modules.Auth.Ports;

namespace API.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class LoginController : ControllerBase
{
    private readonly IAccountLoginFacade loginFacade;

    public LoginController(IAccountLoginFacade loginFacade)
    {
        this.loginFacade = loginFacade;
    }

    [HttpPost("login")]
    public async Task<string> Login(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var login = new Login
        {
            Email = loginRequest.Email,
            Password = loginRequest.Password,
        };
        var accessToken = await loginFacade.HandleAsync(login, cancellationToken);

        return accessToken;
    }
}