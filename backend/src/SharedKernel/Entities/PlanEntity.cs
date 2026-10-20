using System.ComponentModel.DataAnnotations;

namespace SharedKernel.Entities;

public class PlanEntity
{
    [Key]
    public Guid Id { get; set; }

    public required string Code { get; set; }

    public required string Name { get; set; }

    public bool IsDefault { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual ICollection<PlanLimitEntity> Limits { get; set; } = new List<PlanLimitEntity>();
}
