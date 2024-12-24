using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbMigration.migrations
{
    /// <inheritdoc />
    public partial class In2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "hash",
                table: "EthBlocksEntities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EthSwapsETH_USDEntities",
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
                    table.PrimaryKey("PK_EthSwapsETH_USDEntities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EthSwapsETH_USDEntities_blockNumberInt",
                table: "EthSwapsETH_USDEntities",
                column: "blockNumberInt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EthSwapsETH_USDEntities");

            migrationBuilder.DropColumn(
                name: "hash",
                table: "EthBlocksEntities");
        }
    }
}
