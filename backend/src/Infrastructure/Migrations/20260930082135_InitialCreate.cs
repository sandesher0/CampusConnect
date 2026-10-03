<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    ProfileImageUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AccountVerifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailedLoginAttempts = table.Column<int>(type: "integer", nullable: false),
                    RefreshToken = table.Column<List<string>>(type: "text[]", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Community",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommunityName = table.Column<string>(type: "text", nullable: false),
                    CommunityType = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Community", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Community_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunityMember",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityMember_Community_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Community",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunityMember_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    EventDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EventEndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Visibility = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    CreateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_Community_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Community",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Event_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_UserId",
                table: "Account",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_Username",
                table: "Account",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Community_CreatedBy",
                table: "Community",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityMember_CommunityId",
                table: "CommunityMember",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityMember_UserId",
                table: "CommunityMember",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_CommunityId",
                table: "Event",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_CreatedBy",
                table: "Event",
                column: "CreatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "CommunityMember");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "Community");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
=======
﻿    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore.Migrations;

    #nullable disable

    namespace Infrastructure.Migrations
    {
        /// <inheritdoc />
        public partial class InitialCreate : Migration
        {
            /// <inheritdoc />
            protected override void Up(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.CreateTable(
                    name: "User",
                    columns: table => new
                    {
                        Id = table.Column<Guid>(type: "uuid", nullable: false),
                        Email = table.Column<string>(type: "text", nullable: false),
                        FirstName = table.Column<string>(type: "text", nullable: false),
                        LastName = table.Column<string>(type: "text", nullable: false),
                        PhoneNumber = table.Column<string>(type: "text", nullable: true),
                        ProfileImageUrl = table.Column<string>(type: "text", nullable: true),
                        CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                        UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                        DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_User", x => x.Id);
                    });

                migrationBuilder.CreateTable(
                    name: "Account",
                    columns: table => new
                    {
                        Id = table.Column<Guid>(type: "uuid", nullable: false),
                        UserId = table.Column<Guid>(type: "uuid", nullable: false),
                        Username = table.Column<string>(type: "text", nullable: false),
                        PasswordHash = table.Column<string>(type: "text", nullable: false),
                        IsActive = table.Column<bool>(type: "boolean", nullable: false),
                        AccountVerifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                        FailedLoginAttempts = table.Column<int>(type: "integer", nullable: false),
                        RefreshToken = table.Column<List<string>>(type: "text[]", nullable: true),
                        CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                        ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                        DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Account", x => x.Id);
                        table.ForeignKey(
                            name: "FK_Account_User_UserId",
                            column: x => x.UserId,
                            principalTable: "User",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateTable(
                    name: "Community",
                    columns: table => new
                    {
                        Id = table.Column<Guid>(type: "uuid", nullable: false),
                        CommunityName = table.Column<string>(type: "text", nullable: false),
                        CommunityType = table.Column<int>(type: "integer", nullable: false),
                        CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                        Status = table.Column<int>(type: "integer", nullable: false),
                        CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                        UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                        DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Community", x => x.Id);
                        table.ForeignKey(
                            name: "FK_Community_User_CreatedBy",
                            column: x => x.CreatedBy,
                            principalTable: "User",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateTable(
                    name: "CommunityMember",
                    columns: table => new
                    {
                        Id = table.Column<Guid>(type: "uuid", nullable: false),
                        CommunityId = table.Column<Guid>(type: "uuid", nullable: false),
                        UserId = table.Column<Guid>(type: "uuid", nullable: false),
                        MemberType = table.Column<int>(type: "integer", nullable: false),
                        CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                        ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                        DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_CommunityMember", x => x.Id);
                        table.ForeignKey(
                            name: "FK_CommunityMember_Community_CommunityId",
                            column: x => x.CommunityId,
                            principalTable: "Community",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                        table.ForeignKey(
                            name: "FK_CommunityMember_User_UserId",
                            column: x => x.UserId,
                            principalTable: "User",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateTable(
                    name: "Event",
                    columns: table => new
                    {
                        Id = table.Column<Guid>(type: "uuid", nullable: false),
                        Title = table.Column<string>(type: "text", nullable: false),
                        Description = table.Column<string>(type: "text", nullable: false),
                        Location = table.Column<string>(type: "text", nullable: false),
                        CommunityId = table.Column<Guid>(type: "uuid", nullable: false),
                        CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                        EventDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                        EventEndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                        Visibility = table.Column<int>(type: "integer", nullable: false),
                        Category = table.Column<int>(type: "integer", nullable: false),
                        CreateAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                        UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                        DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Event", x => x.Id);
                        table.ForeignKey(
                            name: "FK_Event_Community_CommunityId",
                            column: x => x.CommunityId,
                            principalTable: "Community",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                        table.ForeignKey(
                            name: "FK_Event_User_CreatedBy",
                            column: x => x.CreatedBy,
                            principalTable: "User",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateIndex(
                    name: "IX_Account_UserId",
                    table: "Account",
                    column: "UserId");

                migrationBuilder.CreateIndex(
                    name: "IX_Account_Username",
                    table: "Account",
                    column: "Username",
                    unique: true);

                migrationBuilder.CreateIndex(
                    name: "IX_Community_CreatedBy",
                    table: "Community",
                    column: "CreatedBy");

                migrationBuilder.CreateIndex(
                    name: "IX_CommunityMember_CommunityId",
                    table: "CommunityMember",
                    column: "CommunityId");

                migrationBuilder.CreateIndex(
                    name: "IX_CommunityMember_UserId",
                    table: "CommunityMember",
                    column: "UserId");

                migrationBuilder.CreateIndex(
                    name: "IX_Event_CommunityId",
                    table: "Event",
                    column: "CommunityId");

                migrationBuilder.CreateIndex(
                    name: "IX_Event_CreatedBy",
                    table: "Event",
                    column: "CreatedBy");
            }

            /// <inheritdoc />
            protected override void Down(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.DropTable(
                    name: "Account");

                migrationBuilder.DropTable(
                    name: "CommunityMember");

                migrationBuilder.DropTable(
                    name: "Event");

                migrationBuilder.DropTable(
                    name: "Community");

                migrationBuilder.DropTable(
                    name: "User");
            }
        }
    }
>>>>>>> 1813b66a63af5ebb9db692e0988e04c1922db681
