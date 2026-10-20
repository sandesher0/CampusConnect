using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanLimitsAndFixReservationIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventReservation_EventId_UserId",
                table: "EventReservation");

            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanLimits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    LimitKey = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanLimits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanLimits_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    ValidFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ValidTo = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPlans_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserPlans_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventReservation_EventId_UserId",
                table: "EventReservation",
                columns: new[] { "EventId", "UserId" },
                unique: true,
                filter: "\"DeletedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PlanLimits_PlanId_LimitKey",
                table: "PlanLimits",
                columns: new[] { "PlanId", "LimitKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plans_Code",
                table: "Plans",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plans_IsDefault",
                table: "Plans",
                column: "IsDefault",
                unique: true,
                filter: "\"IsDefault\" = TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlans_PlanId",
                table: "UserPlans",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlans_UserId_ValidFrom",
                table: "UserPlans",
                columns: new[] { "UserId", "ValidFrom" });

            var freePlanId = new Guid("11111111-1111-1111-1111-111111111111");
            var migrationTime = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

            migrationBuilder.InsertData(
                table: "Plans",
                columns: new[] { "Id", "Code", "Name", "IsDefault", "CreatedAt" },
                values: new object[] { freePlanId, "free", "Free Plan", true, migrationTime });

            migrationBuilder.InsertData(
                table: "PlanLimits",
                columns: new[] { "Id", "PlanId", "LimitKey", "Value", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("22222222-2222-2222-2222-222222222201"), freePlanId, "events.max_total_created", 10, migrationTime },
                    { new Guid("22222222-2222-2222-2222-222222222202"), freePlanId, "events.max_active_created", 5, migrationTime },
                    { new Guid("22222222-2222-2222-2222-222222222203"), freePlanId, "communities.max_created", 3, migrationTime },
                    { new Guid("22222222-2222-2222-2222-222222222204"), freePlanId, "events.max_capacity", 200, migrationTime }
                });

            migrationBuilder.Sql($"""
                INSERT INTO "UserPlans" ("Id", "UserId", "PlanId", "ValidFrom", "ValidTo", "CreatedAt")
                SELECT gen_random_uuid(), u."Id", '{freePlanId}', '{migrationTime:O}', NULL, '{migrationTime:O}'
                FROM "User" u
                WHERE NOT EXISTS (
                    SELECT 1 FROM "UserPlans" up WHERE up."UserId" = u."Id"
                );
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanLimits");

            migrationBuilder.DropTable(
                name: "UserPlans");

            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.DropIndex(
                name: "IX_EventReservation_EventId_UserId",
                table: "EventReservation");

            migrationBuilder.CreateIndex(
                name: "IX_EventReservation_EventId_UserId",
                table: "EventReservation",
                columns: new[] { "EventId", "UserId" },
                unique: true);
        }
    }
}
