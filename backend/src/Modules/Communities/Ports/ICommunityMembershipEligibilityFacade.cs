namespace Modules.Communities.Facades;

public interface ICommunityMembershipEligibilityFacade
{
    Task HandleAsync(Guid communityId, Guid userId, CancellationToken cancellationToken);
}