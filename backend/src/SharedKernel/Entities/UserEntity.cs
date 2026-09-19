namespace SharedKernel.Entities;

public class UserEntity

{
    public Guid Id { get; set; }

    public required string Email { get; set; }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset DeletedAt { get; set; }

    public virtual AccountEntity Account { get; set; } = default!;
}
