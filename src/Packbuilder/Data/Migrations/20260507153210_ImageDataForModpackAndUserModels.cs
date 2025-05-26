using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Packbuilder.Migrations
{
    /// <inheritdoc />
    public partial class ImageDataForModpackAndUserModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avatar",
                table: "modpack");

            migrationBuilder.RenameColumn(
                name: "avatar",
                table: "users",
                newName: "image_value");

            migrationBuilder.AddColumn<int>(
                name: "image_type",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "image_type",
                table: "modpack",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "image_value",
                table: "modpack",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_email",
                table: "users");

            migrationBuilder.DropColumn(
                name: "image_type",
                table: "users");

            migrationBuilder.DropColumn(
                name: "image_type",
                table: "modpack");

            migrationBuilder.DropColumn(
                name: "image_value",
                table: "modpack");

            migrationBuilder.RenameColumn(
                name: "image_value",
                table: "users",
                newName: "avatar");

            migrationBuilder.AddColumn<string>(
                name: "avatar",
                table: "modpack",
                type: "text",
                nullable: true);
        }
    }
}
