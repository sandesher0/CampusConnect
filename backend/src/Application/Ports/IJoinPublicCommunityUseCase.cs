namespace Application.Ports;
public interface IJoinPublicCommunityUseCase
{
    Task HandleAsync(Guid communityId , Guid accountId , CancellationToken cancellationToken );
}