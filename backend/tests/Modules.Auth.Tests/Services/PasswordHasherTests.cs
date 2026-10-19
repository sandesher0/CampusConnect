// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Auth.Services
namespace Modules.Auth.Tests.Services;

using Modules.Auth.Services;
using Shouldly;
using Xunit;

public class PasswordHasherTests
{
    private readonly PasswordHasher sut = new();

    [Fact]
    public async Task HashPassword_ValidPassword_GeneratesTwoPartSaltedHash()
    {
        var password = "SecurePassword123!";

        var hash = await sut.HashPassword(password);

        hash.ShouldNotBeNullOrWhiteSpace();
        var parts = hash.Split('.');
        parts.Length.ShouldBe(2);
        Convert.FromBase64String(parts[0]).Length.ShouldBe(16);
        Convert.FromBase64String(parts[1]).Length.ShouldBe(32);
    }

    [Fact]
    public async Task HashPassword_SamePasswordHashedTwice_ProducesDifferentHashesDueToSalt()
    {
        var password = "SamePassword456!";

        var hash1 = await sut.HashPassword(password);
        var hash2 = await sut.HashPassword(password);

        hash1.ShouldNotBe(hash2);
    }

    [Fact]
    public async Task ComparePassword_MatchingPassword_ReturnsTrue()
    {
        var password = "MySecretPassword!";
        var hash = await sut.HashPassword(password);

        var isValid = await sut.ComparePassword(password, hash);

        isValid.ShouldBeTrue();
    }

    [Fact]
    public async Task ComparePassword_WrongPassword_ReturnsFalse()
    {
        var hash = await sut.HashPassword("CorrectPassword");

        var isValid = await sut.ComparePassword("WrongPassword", hash);

        isValid.ShouldBeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-valid-hash")]
    [InlineData("singlepart")]
    [InlineData("one.two.three")]
    public async Task ComparePassword_MalformedStoredHash_ReturnsFalse(string malformedHash)
    {
        var isValid = await sut.ComparePassword("AnyPassword", malformedHash);

        isValid.ShouldBeFalse();
    }
}
