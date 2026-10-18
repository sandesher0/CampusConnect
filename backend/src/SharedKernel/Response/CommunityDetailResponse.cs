using SharedKernel.Constants;

namespace SharedKernel.Response;

public sealed record CommunityDetailResponse
{
    public required Guid CommunityId { get; init; }
    public required string CommunityName { get; init; }
    public required CommunityType CommunityType { get; init; }
    public required Guid CreatedBy { get; init; }

    public required PagedResult<CommunityMemberResponse> CommunityMembers { get; init; }
}