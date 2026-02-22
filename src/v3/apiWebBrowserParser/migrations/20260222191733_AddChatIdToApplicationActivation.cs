using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apiWebBrowserParser.migrations
{
    /// <inheritdoc />
    public partial class AddChatIdToApplicationActivation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChatId",
                table: "ApplicationActivations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "ApplicationActivations");
        }
    }
}
