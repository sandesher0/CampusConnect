using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SharedKernel.Entities;

public class RefreshTokenEntity
{
    [Key]
    public required Guid Id { get; set; }

    public required Guid AccountId { get; set; }

    [ForeignKey(nameof(AccountId))]
    public AccountEntity Account { get; set; } = default!;

    public required string TokenHash { get; set; }
    public required DateTimeOffset ExpiresAtUtc { get; set; }
    public required DateTimeOffset CreatedAtUtc { get; set; }
}
