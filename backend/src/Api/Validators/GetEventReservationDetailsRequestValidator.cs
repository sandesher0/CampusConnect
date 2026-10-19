// Ref: workflow.md §0 AI Development Guidelines | Feature: Validation
namespace Api.Validators;

using Api.Requests;
using FluentValidation;

public class GetEventReservationDetailsRequestValidator : AbstractValidator<GetEventReservationDetailsRequest>
{
    public GetEventReservationDetailsRequestValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("EventId is required");
    }
}
