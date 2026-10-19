// Ref: workflow.md §0 AI Development Guidelines | Feature: Validation
namespace Api.Validators;

using Api.Requests;
using FluentValidation;

public class CancelEventReservationRequestValidator : AbstractValidator<CancelEventReservationRequest>
{
    public CancelEventReservationRequestValidator()
    {
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("EventId is required");
    }
}
