namespace Api.Requests;

public class AccountRegisterRequest
{
    public required string Username { get; init; }

    public required string Email { get; init; }

    public required string Password { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public string? PhoneNumber { get; init; }
}
