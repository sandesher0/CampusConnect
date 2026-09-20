using Microsoft.EntityFrameworkCore;
using Modules.Events.Domain;
using Modules.Users.Domain;
using SharedKernel.Entities;

namespace Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AccountEntity> Account => Set<AccountEntity>();
    public DbSet<UserEntity> User => Set<UserEntity>();
    public DbSet<CommunityEntity> Community => Set<CommunityEntity>();
    public DbSet<CommunityMemberEntity> CommunityMember => Set<CommunityMemberEntity>();
    public DbSet<EventEntity> Event => Set<EventEntity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        builder.Entity<AccountEntity>()
    .HasIndex(account => account.Username)
    .IsUnique();

    }
}