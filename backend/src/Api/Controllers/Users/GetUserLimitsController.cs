using Application.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Exceptions;
using SharedKernel.Response;

namespace Api.Controllers.Users;

[ApiController]
[Route("api/user")]
public class GetUserLimitsController : ControllerBase
{
    private readonly IGetUserLimitsUseCase getUserLimitsUseCase;

    public GetUserLimitsController(IGetUserLimitsUseCase getUserLimitsUseCase)
    {
        this.getUserLimitsUseCase = getUserLimitsUseCase;
    }

    [Authorize]
    [HttpGet("limits")]
    [ProducesResponseType(typeof(UserLimitsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserLimitsResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        var sub = User.FindFirst("sub")?.Value;
        if (sub is null || !Guid.TryParse(sub, out var accountId))
        {
            throw new UnauthorizedAccessException();
        }

        var response = await getUserLimitsUseCase.HandleAsync(accountId, cancellationToken);
        return Ok(response);
    }
}
