using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class In28 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "price",
                table: "EthSwapEvents");

            migrationBuilder.AddColumn<bool>(
                name: "isBuy",
                table: "EthSwapEvents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "priceEth",
                table: "EthSwapEvents",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isBuy",
                table: "EthSwapEvents");

            migrationBuilder.DropColumn(
                name: "priceEth",
                table: "EthSwapEvents");

            migrationBuilder.AddColumn<string>(
                name: "price",
                table: "EthSwapEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
