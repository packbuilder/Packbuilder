using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packbuilder.Migrations
{
    /// <inheritdoc />
    public partial class MigratedDbForBookmarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_modpack_id",
                table: "Bookmarks",
                column: "modpack_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_modpack_modpack_id",
                table: "Bookmarks",
                column: "modpack_id",
                principalTable: "modpack",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_modpack_modpack_id",
                table: "Bookmarks");

            migrationBuilder.DropIndex(
                name: "IX_Bookmarks_modpack_id",
                table: "Bookmarks");
        }
    }
}
