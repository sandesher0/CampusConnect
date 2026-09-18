using Modules.Auth.Domain;

namespace Modules.Auth.Ports;

public interface ITokenGenerator
{
    string GenerateJwtToken(Account account);
}