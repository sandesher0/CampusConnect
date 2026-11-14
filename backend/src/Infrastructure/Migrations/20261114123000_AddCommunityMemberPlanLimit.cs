using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunityMemberPlanLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var freePlanId = new Guid("11111111-1111-1111-1111-111111111111");
            var limitId = new Guid("22222222-2222-2222-2222-222222222205");
            var migrationTime = new DateTimeOffset(2026, 11, 14, 12, 30, 0, TimeSpan.Zero);

            migrationBuilder.Sql($"""
                INSERT INTO "PlanLimits" ("Id", "PlanId", "LimitKey", "Value", "UpdatedAt")
                SELECT '{limitId}', '{freePlanId}', 'community.max_member', 100, '{migrationTime:O}'
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM "PlanLimits"
                    WHERE "PlanId" = '{freePlanId}'
                      AND "LimitKey" = 'community.max_member'
                );
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "PlanLimits"
                WHERE "Id" = '22222222-2222-2222-2222-222222222205'
                  AND "LimitKey" = 'community.max_member';
            """);
        }
    }
}
