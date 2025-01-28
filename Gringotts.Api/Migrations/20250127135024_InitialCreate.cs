﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gringotts.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BuildingName = table.Column<string>(type: "text", nullable: false),
                    RoomName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientSecrets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ManagementUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSecrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSecrets_Clients_ManagementUserId",
                        column: x => x.ManagementUserId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CardId = table.Column<string>(type: "text", nullable: false),
                    SchoolId = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Affiliation = table.Column<byte>(type: "smallint", nullable: false),
                    Sex = table.Column<byte>(type: "smallint", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogUsers_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Readers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessToken = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Readers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Readers_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LogUserAnalyticsSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogUserAnalyticsSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogUserAnalyticsSet_LogUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "LogUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActiveSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LogUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LogReaderId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveSessions_LogUsers_LogUserId",
                        column: x => x.LogUserId,
                        principalTable: "LogUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActiveSessions_Readers_LogReaderId",
                        column: x => x.LogReaderId,
                        principalTable: "Readers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogReaderAnalyticsSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReaderId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogReaderAnalyticsSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogReaderAnalyticsSet_Readers_ReaderId",
                        column: x => x.ReaderId,
                        principalTable: "Readers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<string>(type: "text", nullable: false),
                    LogReaderId = table.Column<Guid>(type: "uuid", nullable: false),
                    LogUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CardId = table.Column<string>(type: "text", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionLogs_LogUsers_LogUserId",
                        column: x => x.LogUserId,
                        principalTable: "LogUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SessionLogs_Readers_LogReaderId",
                        column: x => x.LogReaderId,
                        principalTable: "Readers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LogReaderId = table.Column<Guid>(type: "uuid", nullable: false),
                    LogUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_LogUsers_LogUserId",
                        column: x => x.LogUserId,
                        principalTable: "LogUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sessions_Readers_LogReaderId",
                        column: x => x.LogReaderId,
                        principalTable: "Readers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveSessions_LogReaderId",
                table: "ActiveSessions",
                column: "LogReaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveSessions_LogUserId",
                table: "ActiveSessions",
                column: "LogUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSecrets_ManagementUserId",
                table: "ClientSecrets",
                column: "ManagementUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientSecrets_Username",
                table: "ClientSecrets",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogReaderAnalyticsSet_ReaderId",
                table: "LogReaderAnalyticsSet",
                column: "ReaderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogUserAnalyticsSet_UserId",
                table: "LogUserAnalyticsSet",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogUsers_DepartmentId",
                table: "LogUsers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Readers_LocationId",
                table: "Readers",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionLogs_LogReaderId",
                table: "SessionLogs",
                column: "LogReaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionLogs_LogUserId",
                table: "SessionLogs",
                column: "LogUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_LogReaderId",
                table: "Sessions",
                column: "LogReaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_LogUserId",
                table: "Sessions",
                column: "LogUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveSessions");

            migrationBuilder.DropTable(
                name: "ClientSecrets");

            migrationBuilder.DropTable(
                name: "LogReaderAnalyticsSet");

            migrationBuilder.DropTable(
                name: "LogUserAnalyticsSet");

            migrationBuilder.DropTable(
                name: "SessionLogs");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "LogUsers");

            migrationBuilder.DropTable(
                name: "Readers");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
