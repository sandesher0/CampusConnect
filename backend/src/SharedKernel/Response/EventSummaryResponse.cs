using SharedKernel.Constants;

namespace SharedKernel.Response;

// Compact representation used when rendering an event list.
public sealed record EventSummaryResponse
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Location { get; init; }
    public required DateTimeOffset EventDate { get; init; }
    public required DateTimeOffset EventEndDate { get; init; }
    public required EventVisibility Visibility { get; init; }
    public required EventCategory Category { get; init; }
}
