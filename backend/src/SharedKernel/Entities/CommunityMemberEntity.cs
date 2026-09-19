using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SharedKernel.Constants;
namespace SharedKernel.Entities;

public class CommunityMemberEntity
{
    [Key]
    public required Guid Id { get; set; }
    public required Guid CommunityId { get; set; }
    public required Guid UserId { get; set; }
    public required MemberType MemberType { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    [ForeignKey(nameof(CommunityId))]
    public virtual CommunityEntity Community { get; set; } = default!;
    [ForeignKey(nameof(UserId))]
    public virtual UserEntity User { get; set; } = default!;
}