// Ref: workflow.md §0 AI Development Guidelines | Feature: Validation
namespace Api.Validators;

using Api.Requests;
using FluentValidation;

public class JoinPublicCommunityRequestValidator : AbstractValidator<JoinPublicCommunityRequest>
{
    public JoinPublicCommunityRequestValidator()
    {
        RuleFor(x => x.CommunityId)
            .NotEmpty().WithMessage("CommunityId is required");
    }
}
