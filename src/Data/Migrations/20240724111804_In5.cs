using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class In5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EthBlock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    baseFeePerGas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gasLimit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gasUsed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    timestamp = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthBlock", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EthTrxOther",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    blockHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    blockNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    yParity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    transactionIndex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nonce = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    input = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    r = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    s = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    chainId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    v = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    maxPriorityFeePerGas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    from = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    to = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    maxFeePerGas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gasPrice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    maxFeePerBlobGas = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthTrxOther", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EthBlock");

            migrationBuilder.DropTable(
                name: "EthTrxOther");
        }
    }
}
