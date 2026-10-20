namespace SharedKernel.Domain;

public class PlanLimit
{
    public Guid Id { get; set; }
    public Guid PlanId { get; set; }
    public required string LimitKey { get; set; }
    public int? Value { get; set; }
}
