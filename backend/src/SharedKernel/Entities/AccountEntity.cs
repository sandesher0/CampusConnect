using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedKernel.Entities;

public class AccountEntity
{
    [Key]
    public required Guid Id { get; set; }

    public required Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual UserEntity User { get; set; } = default!;

    public string Username { get; set; } = default!;


    public string PasswordHash { get; set; } = default!;

    public bool IsActive { get; set; } = true;
    public DateTimeOffset? AccountVerifiedAt { get; set; }


    public int FailedLoginAttempts { get; set; }
    public List<string>? RefreshToken { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ModifiedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

}