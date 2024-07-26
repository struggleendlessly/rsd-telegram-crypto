using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class In9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "hash",
                table: "EthTrainData",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contractAddress",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "logs",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_EthTrainData_hash",
                table: "EthTrainData",
                column: "hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EthBlock_numberInt",
                table: "EthBlock",
                column: "numberInt",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EthTrainData_hash",
                table: "EthTrainData");

            migrationBuilder.DropIndex(
                name: "IX_EthBlock_numberInt",
                table: "EthBlock");

            migrationBuilder.DropColumn(
                name: "contractAddress",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "logs",
                table: "EthTrainData");

            migrationBuilder.AlterColumn<string>(
                name: "hash",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
