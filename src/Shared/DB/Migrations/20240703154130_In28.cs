using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.DB.Migrations
{
    /// <inheritdoc />
    public partial class In28 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GyosaRawData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarketCap = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalBribeAttempts = table.Column<int>(type: "int", nullable: false),
                    SuccessfulBribes = table.Column<int>(type: "int", nullable: false),
                    TotalBribeAttemptsETH = table.Column<double>(type: "float", nullable: false),
                    SuccessfulBribesETH = table.Column<double>(type: "float", nullable: false),
                    Controling = table.Column<double>(type: "float", nullable: false),
                    CA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimePosted = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GyosaRawData", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GyosaRawData");
        }
    }
}
