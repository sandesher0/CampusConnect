using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace API.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        this.logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogInformation("An Exception occurred: {Message}", exception.Message);

        if (exception is AccountAlreadyExistsException)
        {
            httpContext.Response.StatusCode =
              StatusCodes.Status409Conflict;

            await Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Account Already Exists",
                detail: exception.Message).ExecuteAsync(httpContext);
            return true;
        }

        if (exception is UsernameAlreadyTakenException)
        {
            httpContext.Response.StatusCode =
              StatusCodes.Status409Conflict;

            await Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Username already Taken",
                detail: exception.Message).ExecuteAsync(httpContext);
            return true;
        }

        if (exception is InvalidCredentialsException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Account not found",
                detail: exception.Message
            ).ExecuteAsync(httpContext);
        }

        if (exception is AccountNotFoundException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Account not found",
                detail: exception.Message
            ).ExecuteAsync(httpContext);
        }

        if (exception is UnauthorizedAccessException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Account not found",
                detail: exception.Message
            ).ExecuteAsync(httpContext);
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await Results.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Internal Server Error",
            detail: "An Unexpected Error Occurred"
        ).ExecuteAsync(httpContext);
        return true;
    }
}
