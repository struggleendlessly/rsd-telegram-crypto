using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class In27 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EthSwapEvents_EthTrainData_EthTrainDataId",
                table: "EthSwapEvents");

            migrationBuilder.AlterColumn<int>(
                name: "EthTrainDataId",
                table: "EthSwapEvents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_EthSwapEvents_EthTrainData_EthTrainDataId",
                table: "EthSwapEvents",
                column: "EthTrainDataId",
                principalTable: "EthTrainData",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EthSwapEvents_EthTrainData_EthTrainDataId",
                table: "EthSwapEvents");

            migrationBuilder.AlterColumn<int>(
                name: "EthTrainDataId",
                table: "EthSwapEvents",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EthSwapEvents_EthTrainData_EthTrainDataId",
                table: "EthSwapEvents",
                column: "EthTrainDataId",
                principalTable: "EthTrainData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
