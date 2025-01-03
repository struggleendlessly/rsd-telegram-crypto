using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbMigration.migrations
{
    /// <inheritdoc />
    public partial class In1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EthBlocksEntities",
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
                    gasUsed = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthBlocksEntities", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EthBlocksEntities");
        }
    }
}
