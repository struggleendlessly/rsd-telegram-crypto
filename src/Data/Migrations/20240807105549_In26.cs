using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class In26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EthTrainDataId",
                table: "EthSwapEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EthSwapEvents_EthTrainDataId",
                table: "EthSwapEvents",
                column: "EthTrainDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_EthSwapEvents_EthTrainData_EthTrainDataId",
                table: "EthSwapEvents",
                column: "EthTrainDataId",
                principalTable: "EthTrainData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EthSwapEvents_EthTrainData_EthTrainDataId",
                table: "EthSwapEvents");

            migrationBuilder.DropIndex(
                name: "IX_EthSwapEvents_EthTrainDataId",
                table: "EthSwapEvents");

            migrationBuilder.DropColumn(
                name: "EthTrainDataId",
                table: "EthSwapEvents");
        }
    }
}
