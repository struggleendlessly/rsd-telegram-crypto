using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apiWebBrowserParser.migrations
{
    /// <inheritdoc />
    public partial class In3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Message",
                table: "messagesEntities",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ChatName",
                table: "messagesEntities",
                newName: "Address");

            migrationBuilder.AddColumn<double>(
                name: "MK",
                table: "messagesEntities",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "isBase",
                table: "messagesEntities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isETH",
                table: "messagesEntities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isSolana",
                table: "messagesEntities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MK",
                table: "messagesEntities");

            migrationBuilder.DropColumn(
                name: "isBase",
                table: "messagesEntities");

            migrationBuilder.DropColumn(
                name: "isETH",
                table: "messagesEntities");

            migrationBuilder.DropColumn(
                name: "isSolana",
                table: "messagesEntities");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "messagesEntities",
                newName: "Message");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "messagesEntities",
                newName: "ChatName");
        }
    }
}
