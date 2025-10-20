using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packbuilder.Migrations
{
    /// <inheritdoc />
    public partial class FixedFkInVersionMod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_version_mods_modpack_id",
                table: "version_mods",
                column: "modpack_id");

            migrationBuilder.AddForeignKey(
                name: "FK_version_mods_modpack_modpack_id",
                table: "version_mods",
                column: "modpack_id",
                principalTable: "modpack",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_version_mods_modpack_modpack_id",
                table: "version_mods");

            migrationBuilder.DropIndex(
                name: "IX_version_mods_modpack_id",
                table: "version_mods");
        }
    }
}
