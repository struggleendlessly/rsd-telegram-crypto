using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbMigration.migrations
{
    /// <inheritdoc />
    public partial class In3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EthSwapsETH_TokenEntities",
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
                    isBuyToken = table.Column<bool>(type: "bit", nullable: false),
                    isBuyEth = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthSwapsETH_TokenEntities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EthSwapsETH_TokenEntities_blockNumberInt",
                table: "EthSwapsETH_TokenEntities",
                column: "blockNumberInt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EthSwapsETH_TokenEntities");
        }
    }
}
