using Microsoft.AspNetCore.Diagnostics;
using SharedKernel.Exceptions;

namespace API.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        this.logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "An exception occurred: {Message}",
            exception.Message);

        if (exception is AccountAlreadyExistsException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

            await Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Account Already Exists",
                detail: exception.Message
            ).ExecuteAsync(httpContext);

            return true;
        }

        if (exception is UsernameAlreadyTakenException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

            await Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Username Already Taken",
                detail: exception.Message
            ).ExecuteAsync(httpContext);

            return true;
        }

        if (exception is InvalidCredentialsException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

            await Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Invalid Credentials",
                detail: exception.Message
            ).ExecuteAsync(httpContext);

            return true;
        }

        if (exception is AccountNotFoundException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

            await Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Account Not Found",
                detail: exception.Message
            ).ExecuteAsync(httpContext);

            return true;
        }

        if (exception is UnauthorizedAccessException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

            await Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized",
                detail: exception.Message
            ).ExecuteAsync(httpContext);

            return true;
        }

        if (exception is ForbiddenException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;

            await Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Forbidden",
                detail: exception.Message
            ).ExecuteAsync(httpContext);

            return true;
        }

        if (exception is ConflictException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

            await Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Conflict",
                detail: exception.Message
            ).ExecuteAsync(httpContext);

            return true;
        }

        if (exception is CommunityNotFoundException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

            await Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Community Not Found",
                detail: exception.Message
            ).ExecuteAsync(httpContext);

            return true;
        }

        if (exception is EventNotFoundException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

            await Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Event Not Found",
                detail: exception.Message
            ).ExecuteAsync(httpContext);

            return true;
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await Results.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Internal Server Error",
            detail: "An unexpected error occurred."
        ).ExecuteAsync(httpContext);

        return true;
    }
}
