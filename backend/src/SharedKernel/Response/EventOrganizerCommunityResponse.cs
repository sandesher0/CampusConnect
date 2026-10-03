namespace SharedKernel.Response;

using SharedKernel.Constants;

public sealed class EventOrganizerCommunityResponse
{
    public required Guid CommunityId { get; init; }
    public required string CommunityName { get; init; }
    public required CommunityType CommunityType { get; init; }
}