using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.DB.Migrations
{
    /// <inheritdoc />
    public partial class In14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TellMessageIdBotVerified",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TellMessageIdIsValid",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TellMessageIdNotVerified",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TellMessageIdBotVerified",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "TellMessageIdIsValid",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "TellMessageIdNotVerified",
                table: "TokenInfos");
        }
    }
}
