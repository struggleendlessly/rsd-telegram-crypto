using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.DB.Migrations
{
    /// <inheritdoc />
    public partial class In8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlockNumber",
                table: "TokenInfos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockNumber",
                table: "TokenInfos");
        }
    }
}
