namespace SharedKernel.Response;

public sealed class EventReservationSummaryResponse
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required DateTimeOffset ReservedAt { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public string? ProfileImageUrl { get; init; }
}