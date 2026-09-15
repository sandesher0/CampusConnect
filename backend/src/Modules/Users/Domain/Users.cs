using SharedKernel.Constants;
namespace Modules.Users.Domain;

public class Users
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Username { get; set; } = default!;
    public Department Department { get; set; } = Department.None;
    public string? ProfileUrl { get; set; }
    public string? Bio { get; set; }
    public DateTimeOffset EmailVerifiedAt { get; set; }


}