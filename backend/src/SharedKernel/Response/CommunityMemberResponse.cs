using SharedKernel.Constants;

namespace SharedKernel.Response;

public sealed class CommunityMemberResponse
{
    public required Guid MemberId { get; init; }
    public required Guid CommunityId { get; init; }
    public required Guid UserId { get; init; }
    public required MemberType MemberType { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? ModifiedAt { get; init; }
    public required CommunityUserResponse User { get; init; }
}