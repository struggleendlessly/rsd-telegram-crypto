using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class In24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeadBlockNumber",
                table: "EthTrainData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "isDead",
                table: "EthTrainData",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "isDeadInt",
                table: "EthTrainData",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeadBlockNumber",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "isDead",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "isDeadInt",
                table: "EthTrainData");
        }
    }
}
