using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class In32 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EthSwapEventsETHUSD",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pairAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    txsHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    from = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    to = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EthIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EthOut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenIn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenOut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    priceEth = table.Column<double>(type: "float", nullable: false),
                    isBuy = table.Column<bool>(type: "bit", nullable: false),
                    blockNumberInt = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthSwapEventsETHUSD", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EthSwapEventsETHUSD");
        }
    }
}
