using SharedKernel.Constants;

namespace SharedKernel.Response;

// Full representation used by the event detail page.
public sealed record EventDetailResponse
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Location { get; init; }
    public required Guid CommunityId { get; init; }
    public required Guid CreatedBy { get; init; }
    public required DateTimeOffset EventDate { get; init; }
    public required DateTimeOffset EventEndDate { get; init; }
    public required EventVisibility Visibility { get; init; }
    public required EventCategory Category { get; init; }
}
