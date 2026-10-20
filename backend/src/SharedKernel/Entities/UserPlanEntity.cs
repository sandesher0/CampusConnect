using System.ComponentModel.DataAnnotations;

namespace SharedKernel.Entities;

public class UserPlanEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid PlanId { get; set; }

    public DateTimeOffset ValidFrom { get; set; }

    public DateTimeOffset? ValidTo { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual UserEntity? User { get; set; }

    public virtual PlanEntity? Plan { get; set; }
}
