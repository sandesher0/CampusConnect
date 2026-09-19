using SharedKernel.Constants;
namespace Modules.Communities.Domain;

public class Community
{
    public Guid Id { get; set; }
    public required string CommunityName { get; set; }
    public required CommunityType CommunityType { get; set; }
    public required Guid CreatedBy { get; set; }
    public CommunityStatusType Status { get; set; }
}