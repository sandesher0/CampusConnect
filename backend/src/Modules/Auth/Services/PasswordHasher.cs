using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Modules.Auth.Ports;
using System.Security.Cryptography;

namespace Modules.Auth.Services;

public class PasswordHasher : IPasswordHasher
{
    public Task<string> HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        byte[] hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32);

        string storedHash =
            $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";

        return Task.FromResult(storedHash);
    }

    public Task<bool> ComparePassword(string password, string storedHash)
    {
        string[] parts = storedHash.Split('.');

        if (parts.Length != 2)
        {
            return Task.FromResult(false);
        }

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] expectedHash = Convert.FromBase64String(parts[1]);

        byte[] actualHash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32);

        bool isValid = CryptographicOperations.FixedTimeEquals(
            actualHash,
            expectedHash);

        return Task.FromResult(isValid);
    }
}