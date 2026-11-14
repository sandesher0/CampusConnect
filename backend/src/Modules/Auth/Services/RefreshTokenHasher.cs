using System.Security.Cryptography;
using System.Text;

namespace Modules.Auth.Services;

public static class RefreshTokenHasher
{
    public static string Hash(string refreshToken)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(refreshToken);
        var hashBytes = SHA256.HashData(tokenBytes);
        return Convert.ToHexString(hashBytes);
    }
}
