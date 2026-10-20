namespace SharedKernel.Domain;

public class Plan
{
    public Guid Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public bool IsDefault { get; set; }
}
