namespace Modules.Auth.Facades;

using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Modules.Auth.Domain;
using Modules.Auth.Ports;
using Modules.Auth.Options;

public class TokenGenerator : ITokenGenerator
{
    private readonly JwtOptions jwtOptions;

    public TokenGenerator(IOptions<JwtOptions> jwtOptions)
    {
        this.jwtOptions = jwtOptions.Value;
    }
    public string GenerateJwtToken(Account account)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Name, account.Username),
            new(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
        };
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtOptions.SecretKey)
            );

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = credentials,
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience
        };
        var token = new JsonWebTokenHandler().CreateToken(tokenDescriptor);

        return token;
    }
}