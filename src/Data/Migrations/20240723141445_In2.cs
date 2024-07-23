using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class In2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "blockHash",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "blockNumber",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "chainId",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "from",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "gas",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "gasPrice",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "hash",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "input",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "isCustomInputStart",
                table: "EthTrainData",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "maxFeePerBlobGas",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "maxFeePerGas",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "maxPriorityFeePerGas",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "nonce",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "r",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "s",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "to",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "transactionIndex",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "v",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "value",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "yParity",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blockHash",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "blockNumber",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "chainId",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "from",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "gas",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "gasPrice",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "hash",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "input",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "isCustomInputStart",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "maxFeePerBlobGas",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "maxFeePerGas",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "maxPriorityFeePerGas",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "nonce",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "r",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "s",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "to",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "transactionIndex",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "type",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "v",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "value",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "yParity",
                table: "EthTrainData");
        }
    }
}
