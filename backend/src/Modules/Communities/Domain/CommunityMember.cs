using SharedKernel.Constants;

namespace Modules.Communities.Domain;


public class CommunityMember
{
    public Guid Id { get; set; }
    public required Guid CommunityId { get; set; }
    public required Guid UserId {get;set;}
    public required MemberType MemberType { get; set; }
    
}