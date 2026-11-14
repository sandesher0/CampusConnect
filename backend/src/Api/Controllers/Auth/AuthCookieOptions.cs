using Microsoft.AspNetCore.Http;
using Modules.Auth.Constants;

namespace Api.Controllers.Auth;

internal static class AuthCookieOptions
{
    private const string AccessTokenPath = "/";
    private const string RefreshTokenPath = "/api/auth";

    public static CookieOptions CreateAccessToken(DateTimeOffset nowUtc)
    {
        return new CookieOptions
        {
            SameSite = SameSiteMode.Lax,
            HttpOnly = true,
            Secure = true,
            Path = AccessTokenPath,
            Expires = nowUtc.Add(AuthTokenLifetimes.AccessTokenLifetime)
        };
    }

    public static CookieOptions CreateRefreshToken(DateTimeOffset nowUtc)
    {
        return new CookieOptions
        {
            SameSite = SameSiteMode.Lax,
            HttpOnly = true,
            Secure = true,
            Path = RefreshTokenPath,
            Expires = nowUtc.Add(AuthTokenLifetimes.RefreshTokenLifetime)
        };
    }

    public static CookieOptions DeleteAccessToken()
    {
        return new CookieOptions
        {
            SameSite = SameSiteMode.Lax,
            HttpOnly = true,
            Secure = true,
            Path = AccessTokenPath
        };
    }

    public static CookieOptions DeleteRefreshToken()
    {
        return new CookieOptions
        {
            SameSite = SameSiteMode.Lax,
            HttpOnly = true,
            Secure = true,
            Path = RefreshTokenPath
        };
    }
}
