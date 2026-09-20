using SharedKernel.Constants;

namespace Api.Requests;

public class CreateCommunityRequest
{
    public required string CommunityName { get; set; }
    public required CommunityType CommunityType { get; set; }
}