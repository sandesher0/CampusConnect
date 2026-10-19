// Ref: workflow.md §0 AI Development Guidelines | Feature: Requests
namespace Api.Requests;

public class SearchCommunityRequest
{
    public required string CommunityName { get; init; }
}
