using Modules.Auth.Ports;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;


namespace Modules.Auth.Facades;

public class PasswordHasher : IPasswordHasher
{
    public async Task<string> HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: password!,
        salt: salt,
        prf: KeyDerivationPrf.HMACSHA256,
        iterationCount: 100000,
        numBytesRequested: 256 / 8));

        return hashed;
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