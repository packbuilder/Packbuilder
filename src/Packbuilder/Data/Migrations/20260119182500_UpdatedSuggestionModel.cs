using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packbuilder.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSuggestionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isOutdated",
                table: "suggestions");

            migrationBuilder.AddColumn<int>(
                name: "state",
                table: "suggestions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "state",
                table: "suggestions");

            migrationBuilder.AddColumn<bool>(
                name: "isOutdated",
                table: "suggestions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
