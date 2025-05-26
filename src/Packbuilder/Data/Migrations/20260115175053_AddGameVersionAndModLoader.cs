using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packbuilder.Migrations
{
    /// <inheritdoc />
    public partial class AddGameVersionAndModLoader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "gameVersion",
                table: "versions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "modLoader",
                table: "versions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "gameVersion",
                table: "suggestions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "modLoader",
                table: "suggestions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "gameVersion",
                table: "versions");

            migrationBuilder.DropColumn(
                name: "modLoader",
                table: "versions");

            migrationBuilder.DropColumn(
                name: "gameVersion",
                table: "suggestions");

            migrationBuilder.DropColumn(
                name: "modLoader",
                table: "suggestions");
        }
    }
}
