using Microsoft.EntityFrameworkCore;
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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        builder.Entity<AccountEntity>()
    .HasIndex(account => account.Username)
    .IsUnique();

    }
}