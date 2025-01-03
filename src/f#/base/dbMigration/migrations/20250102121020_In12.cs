using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbMigration.migrations
{
    /// <inheritdoc />
    public partial class In12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressToken0",
                table: "EthTokenInfoEntities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AddressToken1",
                table: "EthTokenInfoEntities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressToken0",
                table: "EthTokenInfoEntities");

            migrationBuilder.DropColumn(
                name: "AddressToken1",
                table: "EthTokenInfoEntities");
        }
    }
}
