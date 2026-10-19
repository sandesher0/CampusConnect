// Ref: workflow.md §0 AI Development Guidelines | Feature: Validation
namespace Api.Validators;

using Api.Requests;
using FluentValidation;

public class SearchCommunityRequestValidator : AbstractValidator<SearchCommunityRequest>
{
    public SearchCommunityRequestValidator()
    {
        RuleFor(x => x.CommunityName)
            .NotEmpty().WithMessage("CommunityName is required")
            .MaximumLength(100).WithMessage("CommunityName must not exceed 100 characters");
    }
}
