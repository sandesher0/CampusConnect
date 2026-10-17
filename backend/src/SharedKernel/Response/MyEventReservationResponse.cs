using SharedKernel.Constants;

namespace SharedKernel.Response;

public sealed class MyEventReservationResponse
{
    public required Guid ReservationId { get; init; }
    public required Guid EventId { get; init; }
    public required DateTimeOffset ReservedAt { get; init; }

    // Event details
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Location { get; init; }
    public required DateTimeOffset EventDate { get; init; }
    public required DateTimeOffset EventEndDate { get; init; }
    public required EventVisibility Visibility { get; init; }
    public required EventCategory Category { get; init; }
    public required Guid CommunityId { get; init; }
}
