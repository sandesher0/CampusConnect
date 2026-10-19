// Ref: workflow.md §0 AI Development Guidelines | Feature: Validation
namespace Api.Validators;

using Api.Requests;
using FluentValidation;

public class EventRequestValidator : AbstractValidator<EventRequest>
{
    public EventRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required")
            .MinimumLength(2).WithMessage("Location must be at least 2 characters");

        RuleFor(x => x.CommunityId)
            .NotEmpty().WithMessage("CommunityId is required");

        RuleFor(x => x.EventDate)
            .NotEmpty().WithMessage("Start date is required");

        RuleFor(x => x.EventEndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThan(x => x.EventDate).WithMessage("End date must be after start date");

        RuleFor(x => x.Visibility)
            .IsInEnum().WithMessage("Invalid event visibility");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid event category");
    }
}
