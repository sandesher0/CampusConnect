namespace Modules.Auth.Constants;

public static class AuthTokenLifetimes
{
    public static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(30);
    public static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);
}
