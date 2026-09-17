namespace SharedKernel.Entities;

public class AccountEntity
{
    public Guid Id { get; set; }
    public required string Email { get; set; } = default!;
    public required string Username { get; set; } = default!;
    public required string PasswordHash { get; set; } = default!;
    public DateTimeOffset AccountVerifiedAt { get; set; }
    public List<string>? RefreshToken { get; set; }

}