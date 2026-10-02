using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packbuilder.Migrations
{
    /// <inheritdoc />
    public partial class NameFieldOnModModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "mods",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "name",
                table: "mods");
        }
    }
}
