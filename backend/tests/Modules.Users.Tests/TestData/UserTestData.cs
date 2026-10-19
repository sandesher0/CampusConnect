// Ref: workflow.md §0 AI Development Guidelines | Feature: TestData
namespace Modules.Users.Tests.TestData;

using Modules.Users.Domain;
using SharedKernel.Entities;

public static class UserTestData
{
    public static readonly Guid DefaultUserId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

    public static User CreateUser(
        Guid? id = null,
        string email = "testuser@campus.edu",
        string firstName = "Jane",
        string lastName = "Doe",
        string? phoneNumber = "123-456-7890") =>
        new()
        {
            Id = id ?? DefaultUserId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber
        };

    public static UserEntity CreateUserEntity(
        Guid? id = null,
        string email = "testuser@campus.edu",
        string firstName = "Jane",
        string lastName = "Doe",
        string? phoneNumber = "123-456-7890") =>
        new()
        {
            Id = id ?? DefaultUserId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            CreatedAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z")
        };
}
