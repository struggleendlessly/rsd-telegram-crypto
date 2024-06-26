using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.DB.Migrations
{
    /// <inheritdoc />
    public partial class In17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "bitcointalk",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "blog",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "blueCheckmark",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "discord",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "divisor",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "facebook",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "github",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "linkedin",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "reddit",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "slack",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "symbol",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "telegram",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "tokenPriceUSD",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "tokenType",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "totalSupply",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "twitter",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "website",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "wechat",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "whitepaper",
                table: "TokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bitcointalk",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "blog",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "blueCheckmark",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "description",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "discord",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "divisor",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "email",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "facebook",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "github",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "linkedin",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "reddit",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "slack",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "symbol",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "telegram",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "tokenPriceUSD",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "tokenType",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "totalSupply",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "twitter",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "website",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "wechat",
                table: "TokenInfos");

            migrationBuilder.DropColumn(
                name: "whitepaper",
                table: "TokenInfos");
        }
    }
}
