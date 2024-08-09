using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class In30 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "exploitsTS",
                table: "EthTrainData");

            migrationBuilder.AddColumn<string>(
                name: "tsExploits",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "tsFullResponse",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_EthTrainData_blockNumberInt",
                table: "EthTrainData",
                column: "blockNumberInt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EthTrainData_blockNumberInt",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "tsExploits",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "tsFullResponse",
                table: "EthTrainData");

            migrationBuilder.AddColumn<bool>(
                name: "exploitsTS",
                table: "EthTrainData",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
