// Ref: workflow.md §0 AI Development Guidelines | Feature: Validation
namespace Api.Validators;

using Api.Requests;
using FluentValidation;

public class JoinEventRequestValidator : AbstractValidator<JoinEventRequest>
{
    public JoinEventRequestValidator()
    {
        RuleFor(x => x.CommunityId)
            .NotEmpty().WithMessage("CommunityId is required");

        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("EventId is required");
    }
}
