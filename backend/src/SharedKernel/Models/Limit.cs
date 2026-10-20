namespace SharedKernel.Models;

public readonly record struct Limit(int? Value)
{
    public bool IsUnlimited => Value is null;

    public static implicit operator Limit(int? value) => new(value);
}
