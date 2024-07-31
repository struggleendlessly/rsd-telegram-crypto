using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class In17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BalanceOnCreating",
                table: "EthTrainData",
                type: "int",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.AddColumn<DateTime>(
                name: "walletCreated",
                table: "EthTrainData",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BalanceOnCreating",
                table: "EthTrainData");

            migrationBuilder.DropColumn(
                name: "walletCreated",
                table: "EthTrainData");
        }
    }
}
