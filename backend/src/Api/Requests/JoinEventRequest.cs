// Ref: workflow.md §0 AI Development Guidelines | Feature: Requests
namespace Api.Requests;

public class JoinEventRequest
{
    public required Guid CommunityId { get; init; }
    public required Guid EventId { get; init; }
}
