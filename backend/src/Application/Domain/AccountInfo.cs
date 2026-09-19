namespace Application.Domain;

public class AccountInfo
{
    public required Guid Id { get; set; }

    public required Guid UserId { get; set; }
    public required string Username { get; set; } = default!;
    public DateTimeOffset? AccountVerifiedAt { get; set; }

    public required UserInfo User { get; set; }

}