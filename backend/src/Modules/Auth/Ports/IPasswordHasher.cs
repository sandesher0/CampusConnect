namespace Modules.Auth.Ports;


public interface IPasswordHasher
{
    Task<string> HashPassword(string password);
    Task<bool> ComparePassword(string passwordHash, string originalPassword);
}