// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Auth.Services
namespace Modules.Auth.Tests.Services;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Modules.Auth.Options;
using Modules.Auth.Services;
using Modules.Auth.Tests.TestData;
using Shouldly;
using System.Text;
using Xunit;

public class TokenGeneratorTests
{
    private readonly JwtOptions jwtOptions = new()
    {
        Issuer = "CampusConnectAuth",
        Audience = "CampusConnectApp",
        SecretKey = "SuperSecretKeyForTestingPurposes1234567890!#"
    };

    [Fact]
    public void GenerateJwtToken_ValidAccount_IncludesRequiredClaimsAndValidSignature()
    {
        var optionsMock = Options.Create(jwtOptions);
        var sut = new TokenGenerator(optionsMock);
        var account = AuthTestData.CreateAccount(
            id: Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            username: "johndoe");

        var tokenString = sut.GenerateJwtToken(account);

        tokenString.ShouldNotBeNullOrWhiteSpace();

        var handler = new JsonWebTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        var result = handler.ValidateToken(tokenString, validationParameters);
        result.IsValid.ShouldBeTrue();
        result.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Sub)?.Value.ShouldBe(account.Id.ToString());
        result.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Name)?.Value.ShouldBe(account.Username);
    }

    [Fact]
    public void GenerateJwtToken_TamperedToken_FailsValidation()
    {
        var optionsMock = Options.Create(jwtOptions);
        var sut = new TokenGenerator(optionsMock);
        var account = AuthTestData.CreateAccount();

        var tokenString = sut.GenerateJwtToken(account);
        var tamperedToken = tokenString[..^5] + "XXXXX";

        var handler = new JsonWebTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
            ValidateLifetime = false
        };

        var result = handler.ValidateToken(tamperedToken, validationParameters);
        result.IsValid.ShouldBeFalse();
    }
}
