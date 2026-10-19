// Ref: workflow.md §0 AI Development Guidelines | Feature: Validation Filter
namespace Api.Filters;

using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class FluentValidationActionFilter : IAsyncActionFilter
{
    private readonly IServiceProvider serviceProvider;

    public FluentValidationActionFilter(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var actionArgument in context.ActionArguments.Values)
        {
            if (actionArgument is null)
            {
                continue;
            }

            var argumentType = actionArgument.GetType();
            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

            if (serviceProvider.GetService(validatorType) is IValidator validator)
            {
                var validationContext = new ValidationContext<object>(actionArgument);
                var validationResult = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );

                    var problemDetails = new HttpValidationProblemDetails(errors)
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "One or more validation errors occurred.",
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
                    };

                    context.Result = new BadRequestObjectResult(problemDetails);
                    return;
                }
            }
        }

        await next();
    }
}
