// Ref: workflow.md §0 AI Development Guidelines | Feature: Requests
namespace Api.Requests;

public class GetEventRequest
{
    public required Guid EventId { get; init; }
}
