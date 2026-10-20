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
    public DbSet<EventReservationEntity> EventReservation => Set<EventReservationEntity>();
    public DbSet<PlanEntity> Plans => Set<PlanEntity>();
    public DbSet<PlanLimitEntity> PlanLimits => Set<PlanLimitEntity>();
    public DbSet<UserPlanEntity> UserPlans => Set<UserPlanEntity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        builder.Entity<AccountEntity>()
            .HasIndex(account => account.Username)
            .IsUnique();

        builder.Entity<EventReservationEntity>()
            .HasIndex(reservation => new { reservation.EventId, reservation.UserId })
            .HasFilter("\"DeletedAt\" IS NULL")
            .IsUnique();

        builder.Entity<PlanEntity>(entity =>
        {
            entity.HasIndex(p => p.Code).IsUnique();
            entity.HasIndex(p => p.IsDefault)
                .HasFilter("\"IsDefault\" = TRUE")
                .IsUnique();
        });

        builder.Entity<PlanLimitEntity>(entity =>
        {
            entity.HasIndex(pl => new { pl.PlanId, pl.LimitKey }).IsUnique();
            entity.HasOne(pl => pl.Plan)
                .WithMany(p => p.Limits)
                .HasForeignKey(pl => pl.PlanId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<UserPlanEntity>(entity =>
        {
            entity.HasIndex(up => new { up.UserId, up.ValidFrom });
            entity.HasOne(up => up.User)
                .WithMany(u => u.UserPlans)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(up => up.Plan)
                .WithMany()
                .HasForeignKey(up => up.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
