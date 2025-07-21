using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apiWebBrowserParser.migrations
{
    /// <inheritdoc />
    public partial class In4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChatTitle",
                table: "messagesEntities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Network",
                table: "messagesEntities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatTitle",
                table: "messagesEntities");

            migrationBuilder.DropColumn(
                name: "Network",
                table: "messagesEntities");
        }
    }
}
