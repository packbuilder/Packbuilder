using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packbuilder.Migrations
{
    /// <inheritdoc />
    public partial class UserUsernameUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_users_name",
                table: "users",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_name",
                table: "users");
        }
    }
}
