namespace Modules.Events.Ports;
public interface ICommunityMembershipChecker
{
    Task<bool> IsClubOfficerAsync(
        Guid communityId,
        Guid userId,
        CancellationToken cancellationToken);
}