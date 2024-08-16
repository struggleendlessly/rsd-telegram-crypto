using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class In34 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "priceEth",
                table: "EthSwapEventsETHUSD",
                newName: "priceEthInUsd");

            migrationBuilder.RenameColumn(
                name: "isBuy",
                table: "EthSwapEventsETHUSD",
                newName: "isBuyEth");

            migrationBuilder.AddColumn<bool>(
                name: "isBuyDai",
                table: "EthSwapEventsETHUSD",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isBuyDai",
                table: "EthSwapEventsETHUSD");

            migrationBuilder.RenameColumn(
                name: "priceEthInUsd",
                table: "EthSwapEventsETHUSD",
                newName: "priceEth");

            migrationBuilder.RenameColumn(
                name: "isBuyEth",
                table: "EthSwapEventsETHUSD",
                newName: "isBuy");
        }
    }
}
