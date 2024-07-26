using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class In10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "decimals",
                table: "EthTrainData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "logo",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "symbol",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "decimals",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "logo",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "name",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "symbol",
                table: "EthTrainData");
        }
    }
}
