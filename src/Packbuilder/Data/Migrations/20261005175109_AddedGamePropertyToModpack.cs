using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packbuilder.Migrations
{
    /// <inheritdoc />
    public partial class AddedGamePropertyToModpack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "game",
                table: "modpack",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "name",
                table: "mods");

            migrationBuilder.DropColumn(
                name: "game",
                table: "modpack");
        }
    }
}
