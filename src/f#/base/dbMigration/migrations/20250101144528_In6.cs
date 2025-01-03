using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbMigration.migrations
{
    /// <inheritdoc />
    public partial class In6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EthTokenInfoEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameLong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameShort = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Decimals = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthTokenInfoEntities", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EthTokenInfoEntities");
        }
    }
}
