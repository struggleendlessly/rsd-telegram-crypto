using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace db.Migrations
{
    /// <inheritdoc />
    public partial class In11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "swapsETH_Token_30MinsEntities",
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
                    priceTokenInUSD_min = table.Column<decimal>(type: "decimal(38,20)", nullable: false),
                    priceTokenInUSD_max = table.Column<decimal>(type: "decimal(38,20)", nullable: false),
                    priceTokenInUSD_avr = table.Column<decimal>(type: "decimal(38,20)", nullable: false),
                    priceETH_USD = table.Column<double>(type: "float", nullable: false),
                    isBuyToken = table.Column<bool>(type: "bit", nullable: false),
                    isBuyEth = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_swapsETH_Token_30MinsEntities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_swapsETH_Token_30MinsEntities_blockNumberEndInt",
                table: "swapsETH_Token_30MinsEntities",
                column: "blockNumberEndInt");

            migrationBuilder.CreateIndex(
                name: "IX_swapsETH_Token_30MinsEntities_blockNumberStartInt",
                table: "swapsETH_Token_30MinsEntities",
                column: "blockNumberStartInt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "swapsETH_Token_30MinsEntities");
        }
    }
}
