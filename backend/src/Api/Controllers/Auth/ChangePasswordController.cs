using Api.Requests;
using Microsoft.AspNetCore.Mvc;
using Modules.Auth.Ports;
using Modules.Auth.Domain;
using System.Security.Claims;

namespace Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class ChangePasswordController : ControllerBase
{
    private readonly IChangePasswordFacade changePasswordFacade;

    public ChangePasswordController(IChangePasswordFacade changePasswordFacade)
    {
        this.changePasswordFacade = changePasswordFacade;
    }

    [HttpPatch("change-password")]
    public async Task<IActionResult> HandleAsync(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var accountId = User.FindFirst("sub")?.Value;

        if (accountId is null)
        {
            throw new UnauthorizedAccessException();
        }

        var changePassword = new ChangePassword
        {
            OldPassword = request.OldPassword,
            NewPassword = request.NewPassword
        };
        await changePasswordFacade.HandleAsync(Guid.Parse(accountId), changePassword, cancellationToken);
        return Ok("Password Changed Successfully");
    }
}