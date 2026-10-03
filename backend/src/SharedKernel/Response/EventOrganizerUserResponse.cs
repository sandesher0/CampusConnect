namespace SharedKernel.Response;

public sealed class EventOrganizerUserResponse
{
    public required Guid Id { get; set; }

    public required string Email { get; set; } = default!;

    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }
}