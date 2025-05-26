using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packbuilder.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdSuggestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_suggestions_users_UserId",
                table: "suggestions");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "suggestions",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_suggestions_UserId",
                table: "suggestions",
                newName: "IX_suggestions_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_suggestions_users_user_id",
                table: "suggestions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_suggestions_users_user_id",
                table: "suggestions");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "suggestions",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_suggestions_user_id",
                table: "suggestions",
                newName: "IX_suggestions_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_suggestions_users_UserId",
                table: "suggestions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
