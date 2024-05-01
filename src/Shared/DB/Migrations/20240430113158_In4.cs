using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.DB.Migrations
{
    /// <inheritdoc />
    public partial class In4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TokenInfoUrls");

            migrationBuilder.AddColumn<string>(
                name: "UrlChart",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UrlOwnersWallet",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UrlToken",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlChart",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "UrlOwnersWallet",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "UrlToken",
                table: "TokenInfos");

            migrationBuilder.CreateTable(
                name: "TokenInfoUrls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TokenInfoId = table.Column<int>(type: "int", nullable: false),
                    UrlChart = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlOwnersWallet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UrlToken = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenInfoUrls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TokenInfoUrls_TokenInfos_TokenInfoId",
                        column: x => x.TokenInfoId,
                        principalTable: "TokenInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TokenInfoUrls_TokenInfoId",
                table: "TokenInfoUrls",
                column: "TokenInfoId");
        }
    }
}
