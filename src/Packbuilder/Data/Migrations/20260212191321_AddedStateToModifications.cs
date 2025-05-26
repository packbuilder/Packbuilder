using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packbuilder.Migrations
{
    /// <inheritdoc />
    public partial class AddedStateToModifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_conflicting",
                table: "modifications");

            migrationBuilder.AddColumn<int>(
                name: "state",
                table: "modifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "state",
                table: "modifications");

            migrationBuilder.AddColumn<bool>(
                name: "is_conflicting",
                table: "modifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
