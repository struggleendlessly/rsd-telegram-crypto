using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class In35 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EthSwapEventsETHUSD_blockNumberInt",
                table: "EthSwapEventsETHUSD");

            migrationBuilder.CreateIndex(
                name: "IX_EthSwapEventsETHUSD_blockNumberInt",
                table: "EthSwapEventsETHUSD",
                column: "blockNumberInt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EthSwapEventsETHUSD_blockNumberInt",
                table: "EthSwapEventsETHUSD");

            migrationBuilder.CreateIndex(
                name: "IX_EthSwapEventsETHUSD_blockNumberInt",
                table: "EthSwapEventsETHUSD",
                column: "blockNumberInt",
                unique: true);
        }
    }
}
