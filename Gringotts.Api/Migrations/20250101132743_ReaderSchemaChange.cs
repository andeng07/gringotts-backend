using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gringotts.Api.Migrations
{
    /// <inheritdoc />
    public partial class ReaderSchemaChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReaderSecrets");

            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "Readers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "RoomName",
                table: "Locations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "Readers");

            migrationBuilder.AlterColumn<string>(
                name: "RoomName",
                table: "Locations",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ReaderSecrets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessToken = table.Column<string>(type: "text", nullable: false),
                    ReaderId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReaderSecrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReaderSecrets_Readers_ReaderId",
                        column: x => x.ReaderId,
                        principalTable: "Readers",
                        principalColumn: "ReferenceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReaderSecrets_ReaderId",
                table: "ReaderSecrets",
                column: "LogReaderId",
                unique: true);
        }
    }
}
