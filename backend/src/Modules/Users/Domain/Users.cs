namespace Modules.Users.Domain;

public class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = default!;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }


}
