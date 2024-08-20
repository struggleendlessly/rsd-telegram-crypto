
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class In37 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EthTokensVolumes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    blockIntStart = table.Column<int>(type: "int", nullable: false),
                    blockIntEnd = table.Column<int>(type: "int", nullable: false),
                    periodInMins = table.Column<int>(type: "int", nullable: false),
                    volumePositiveEth = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    volumeNegativeEth = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    volumeTotalEth = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EthTrainDataId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthTokensVolumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EthTokensVolumes_EthTrainData_EthTrainDataId",
                        column: x => x.EthTrainDataId,
                        principalTable: "EthTrainData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EthTokensVolumes_EthTrainDataId",
                table: "EthTokensVolumes",
                column: "EthTrainDataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EthTokensVolumes");
        }
    }
}
