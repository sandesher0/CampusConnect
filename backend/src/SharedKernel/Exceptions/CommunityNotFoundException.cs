namespace SharedKernel.Exceptions;

public class CommunityNotFoundException : Exception
{
    public CommunityNotFoundException(Guid communityId) : base($"Community {communityId} Not Found")
    {

    }
}