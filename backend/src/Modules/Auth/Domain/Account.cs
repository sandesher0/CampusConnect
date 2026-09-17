namespace Modules.Auth.Domain;

public class Account
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public DateTimeOffset? AccountVerifiedAt { get; set; }
    public List<string>? RefreshToken { get; set; }


}
