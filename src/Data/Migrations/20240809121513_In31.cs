using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class In31 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "amount1out",
                table: "EthSwapEvents",
                newName: "txsHash");

            migrationBuilder.RenameColumn(
                name: "amount1in",
                table: "EthSwapEvents",
                newName: "TokenOut");

            migrationBuilder.RenameColumn(
                name: "amount0out",
                table: "EthSwapEvents",
                newName: "TokenIn");

            migrationBuilder.RenameColumn(
                name: "amount0in",
                table: "EthSwapEvents",
                newName: "EthOut");

            migrationBuilder.AddColumn<string>(
                name: "EthIn",
                table: "EthSwapEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EthIn",
                table: "EthSwapEvents");

            migrationBuilder.RenameColumn(
                name: "txsHash",
                table: "EthSwapEvents",
                newName: "amount1out");

            migrationBuilder.RenameColumn(
                name: "TokenOut",
                table: "EthSwapEvents",
                newName: "amount1in");

            migrationBuilder.RenameColumn(
                name: "TokenIn",
                table: "EthSwapEvents",
                newName: "amount0out");

            migrationBuilder.RenameColumn(
                name: "EthOut",
                table: "EthSwapEvents",
                newName: "amount0in");
        }
    }
}
