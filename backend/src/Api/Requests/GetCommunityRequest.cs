// Ref: workflow.md §0 AI Development Guidelines | Feature: Requests
namespace Api.Requests;

public class GetCommunityRequest
{
    public required Guid CommunityId { get; init; }
}
