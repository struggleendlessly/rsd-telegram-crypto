using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Puppeteer.Console.BlazorUI.Migrations
{
    /// <inheritdoc />
    public partial class AddSaveLoginDataForTelegramChatsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SaveLoginData",
                table: "TelegramChats",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaveLoginData",
                table: "TelegramChats");
        }
    }
}
