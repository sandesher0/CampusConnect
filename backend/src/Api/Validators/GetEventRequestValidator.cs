// Ref: workflow.md §0 AI Development Guidelines | Feature: Validation
namespace Api.Validators;

using Api.Requests;
using FluentValidation;

public class GetEventRequestValidator : AbstractValidator<GetEventRequest>
{
    public GetEventRequestValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("EventId is required");
    }
}
