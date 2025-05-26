using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packbuilder.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_suggestions_modpack_ModpackId",
                table: "suggestions");

            migrationBuilder.DropColumn(
                name: "modpack_slug",
                table: "suggestions");

            migrationBuilder.RenameColumn(
                name: "modLoader",
                table: "versions",
                newName: "mod_loader");

            migrationBuilder.RenameColumn(
                name: "gameVersion",
                table: "versions",
                newName: "game_version");

            migrationBuilder.RenameColumn(
                name: "ModpackId",
                table: "suggestions",
                newName: "modpack_id");

            migrationBuilder.RenameIndex(
                name: "IX_suggestions_ModpackId",
                table: "suggestions",
                newName: "IX_suggestions_modpack_id");

            migrationBuilder.AddForeignKey(
                name: "FK_suggestions_modpack_modpack_id",
                table: "suggestions",
                column: "modpack_id",
                principalTable: "modpack",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_suggestions_modpack_modpack_id",
                table: "suggestions");

            migrationBuilder.RenameColumn(
                name: "mod_loader",
                table: "versions",
                newName: "modLoader");

            migrationBuilder.RenameColumn(
                name: "game_version",
                table: "versions",
                newName: "gameVersion");

            migrationBuilder.RenameColumn(
                name: "modpack_id",
                table: "suggestions",
                newName: "ModpackId");

            migrationBuilder.RenameIndex(
                name: "IX_suggestions_modpack_id",
                table: "suggestions",
                newName: "IX_suggestions_ModpackId");

            migrationBuilder.AddColumn<string>(
                name: "modpack_slug",
                table: "suggestions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_suggestions_modpack_ModpackId",
                table: "suggestions",
                column: "ModpackId",
                principalTable: "modpack",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
