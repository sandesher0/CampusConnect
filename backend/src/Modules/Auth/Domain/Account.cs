namespace Modules.Auth.Domain;

public class Account
{
    public Guid Id { get; set; }

    public required Guid UserId { get; set; }
    public string Username { get; set; } = default!;


    public string PasswordHash { get; set; } = default!;

    public bool IsActive { get; set; } = true;
    public DateTimeOffset? AccountVerifiedAt { get; set; }


    public int FailedLoginAttempts { get; set; }
    public List<string>? RefreshToken { get; set; }


}
