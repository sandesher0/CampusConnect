using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SharedKernel.Constants;

namespace SharedKernel.Entities;

public class CommunityEntity
{
    [Key]
    public required Guid Id { get; set; }
    public required string CommunityName { get; set; }
    public required CommunityType CommunityType { get; set; }
    public required Guid CreatedBy { get; set; }
    public required CommunityStatusType Status { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    [ForeignKey(nameof(CreatedBy))]
    public virtual UserEntity CreatedByUser { get; set; } = default!;
}