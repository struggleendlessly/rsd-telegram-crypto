using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbMigration.Migrations
{
    /// <inheritdoc />
    public partial class In7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "swapsTokensUSDEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    slotNumberInt = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    addressToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    txsHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    from = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    to = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    t0amount = table.Column<double>(type: "float", nullable: false),
                    t1amount = table.Column<double>(type: "float", nullable: false),
                    priceSolInUsd = table.Column<double>(type: "float", nullable: false),
                    isBuyDai = table.Column<bool>(type: "bit", nullable: false),
                    isBuySol = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_swapsTokensUSDEntities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_swapsTokensUSDEntities_slotNumberInt",
                table: "swapsTokensUSDEntities",
                column: "slotNumberInt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "swapsTokensUSDEntities");
        }
    }
}
