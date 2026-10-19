// Ref: workflow.md §0 AI Development Guidelines | Feature: Validation
namespace Api.Validators;

using Api.Requests;
using FluentValidation;
using SharedKernel.Constants;

public class CreateCommunityRequestValidator : AbstractValidator<CreateCommunityRequest>
{
    public CreateCommunityRequestValidator()
    {
        RuleFor(x => x.CommunityName)
            .NotEmpty().WithMessage("Community name is required")
            .MinimumLength(3).WithMessage("Community name must be at least 3 characters")
            .MaximumLength(100).WithMessage("Community name must not exceed 100 characters");

        RuleFor(x => x.CommunityType)
            .IsInEnum().WithMessage("Invalid community type")
            .Must(t => t == CommunityType.Public || t == CommunityType.Private)
            .WithMessage("Community type must be Public or Private");
    }
}
