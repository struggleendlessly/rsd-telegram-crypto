using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.DB.Migrations
{
    /// <inheritdoc />
    public partial class In16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameToken",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameToken",
                table: "TokenInfos");
        }
    }
}
