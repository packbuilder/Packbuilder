using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packbuilder.Migrations
{
    /// <inheritdoc />
    public partial class AddedConflictStateToVersionModAndModificationModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "state",
                table: "modifications",
                newName: "conflict_state");

            migrationBuilder.AddColumn<int>(
                name: "conflict_state",
                table: "version_mods",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "conflict_state",
                table: "version_mods");

            migrationBuilder.RenameColumn(
                name: "conflict_state",
                table: "modifications",
                newName: "state");
        }
    }
}
