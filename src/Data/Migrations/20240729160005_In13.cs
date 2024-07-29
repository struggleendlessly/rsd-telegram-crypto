using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class In13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ABI",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompilerVersion",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConstructorArguments",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContractName",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EVMVersion",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Implementation",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Library",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseType",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptimizationUsed",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Proxy",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Runs",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceCode",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwarmSource",
                table: "EthTrainData",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "exploitsTS",
                table: "EthTrainData",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ABI",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "CompilerVersion",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "ConstructorArguments",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "ContractName",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "EVMVersion",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "Implementation",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "Library",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "LicenseType",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "OptimizationUsed",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "Proxy",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "Runs",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "SourceCode",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "SwarmSource",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "exploitsTS",
                table: "EthTrainData");
        }
    }
}
