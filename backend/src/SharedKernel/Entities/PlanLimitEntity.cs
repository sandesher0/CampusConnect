using System.ComponentModel.DataAnnotations;

namespace SharedKernel.Entities;

public class PlanLimitEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid PlanId { get; set; }

    public required string LimitKey { get; set; }

    public int? Value { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public virtual PlanEntity? Plan { get; set; }
}
