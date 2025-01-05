using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbMigration.Migrations
{
    /// <inheritdoc />
    public partial class In1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "blocksEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    numberHex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    numberInt = table.Column<int>(type: "int", nullable: false),
                    timestampUnix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    timestampNormal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    baseFeePerGas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gasLimit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gasUsed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blocksEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "swapsETH_TokenEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    blockNumberStartInt = table.Column<int>(type: "int", nullable: false),
                    blockNumberEndInt = table.Column<int>(type: "int", nullable: false),
                    pairAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    txsHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    from = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    to = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EthIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EthOut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenOut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    priceTokenInETH = table.Column<double>(type: "float", nullable: false),
                    priceETH_USD = table.Column<double>(type: "float", nullable: false),
                    isBuyToken = table.Column<bool>(type: "bit", nullable: false),
                    isBuyEth = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_swapsETH_TokenEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "swapsETH_USDEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    blockNumberInt = table.Column<int>(type: "int", nullable: false),
                    pairAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    txsHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    from = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    to = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EthIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EthOut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenOut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    priceEthInUsd = table.Column<double>(type: "float", nullable: false),
                    isBuyDai = table.Column<bool>(type: "bit", nullable: false),
                    isBuyEth = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_swapsETH_USDEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tokenInfoEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddressToken = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AddressToken0 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressToken1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressPair = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NameLong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameShort = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Decimals = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tokenInfoEntities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_swapsETH_TokenEntities_blockNumberEndInt",
                table: "swapsETH_TokenEntities",
                column: "blockNumberEndInt");

            migrationBuilder.CreateIndex(
                name: "IX_swapsETH_TokenEntities_blockNumberStartInt",
                table: "swapsETH_TokenEntities",
                column: "blockNumberStartInt");

            migrationBuilder.CreateIndex(
                name: "IX_swapsETH_USDEntities_blockNumberInt",
                table: "swapsETH_USDEntities",
                column: "blockNumberInt");

            migrationBuilder.CreateIndex(
                name: "IX_tokenInfoEntities_AddressPair",
                table: "tokenInfoEntities",
                column: "AddressPair");

            migrationBuilder.CreateIndex(
                name: "IX_tokenInfoEntities_AddressToken",
                table: "tokenInfoEntities",
                column: "AddressToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blocksEntities");

            migrationBuilder.DropTable(
                name: "swapsETH_TokenEntities");

            migrationBuilder.DropTable(
                name: "swapsETH_USDEntities");

            migrationBuilder.DropTable(
                name: "tokenInfoEntities");
        }
    }
}
