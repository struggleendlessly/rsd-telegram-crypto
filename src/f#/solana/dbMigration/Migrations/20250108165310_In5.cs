using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbMigration.Migrations
{
    /// <inheritdoc />
    public partial class In5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TokenOut",
                table: "swapsTokensEntities",
                newName: "tokenOut");

            migrationBuilder.RenameColumn(
                name: "TokenIn",
                table: "swapsTokensEntities",
                newName: "tokenIn");

            migrationBuilder.RenameColumn(
                name: "SolOut",
                table: "swapsTokensEntities",
                newName: "solOut");

            migrationBuilder.RenameColumn(
                name: "SolIn",
                table: "swapsTokensEntities",
                newName: "solIn");

            migrationBuilder.RenameColumn(
                name: "blockNumberStartInt",
                table: "swapsTokensEntities",
                newName: "slotNumberStartInt");

            migrationBuilder.RenameColumn(
                name: "blockNumberEndInt",
                table: "swapsTokensEntities",
                newName: "slotNumberEndInt");

            migrationBuilder.CreateIndex(
                name: "IX_swapsTokensEntities_slotNumberEndInt",
                table: "swapsTokensEntities",
                column: "slotNumberEndInt");

            migrationBuilder.CreateIndex(
                name: "IX_swapsTokensEntities_slotNumberStartInt",
                table: "swapsTokensEntities",
                column: "slotNumberStartInt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_swapsTokensEntities_slotNumberEndInt",
                table: "swapsTokensEntities");

            migrationBuilder.DropIndex(
                name: "IX_swapsTokensEntities_slotNumberStartInt",
                table: "swapsTokensEntities");

            migrationBuilder.RenameColumn(
                name: "tokenOut",
                table: "swapsTokensEntities",
                newName: "TokenOut");

            migrationBuilder.RenameColumn(
                name: "tokenIn",
                table: "swapsTokensEntities",
                newName: "TokenIn");

            migrationBuilder.RenameColumn(
                name: "solOut",
                table: "swapsTokensEntities",
                newName: "SolOut");

            migrationBuilder.RenameColumn(
                name: "solIn",
                table: "swapsTokensEntities",
                newName: "SolIn");

            migrationBuilder.RenameColumn(
                name: "slotNumberStartInt",
                table: "swapsTokensEntities",
                newName: "blockNumberStartInt");

            migrationBuilder.RenameColumn(
                name: "slotNumberEndInt",
                table: "swapsTokensEntities",
                newName: "blockNumberEndInt");
        }
    }
}
