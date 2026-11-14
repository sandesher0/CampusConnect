namespace Modules.Auth.Domain;

public class LoginTokens
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
