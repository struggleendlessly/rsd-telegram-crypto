using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbMigration.Migrations
{
    /// <inheritdoc />
    public partial class In1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "slotsEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    numberInt = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_slotsEntities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_slotsEntities_numberInt",
                table: "slotsEntities",
                column: "numberInt",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "slotsEntities");
        }
    }
}
