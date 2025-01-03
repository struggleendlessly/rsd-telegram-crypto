using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbMigration.migrations
{
    /// <inheritdoc />
    public partial class In4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "blockNumberInt",
                table: "EthSwapsETH_TokenEntities",
                newName: "blockNumberStartInt");

            migrationBuilder.RenameIndex(
                name: "IX_EthSwapsETH_TokenEntities_blockNumberInt",
                table: "EthSwapsETH_TokenEntities",
                newName: "IX_EthSwapsETH_TokenEntities_blockNumberStartInt");

            migrationBuilder.AddColumn<int>(
                name: "blockNumberEndInt",
                table: "EthSwapsETH_TokenEntities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EthSwapsETH_TokenEntities_blockNumberEndInt",
                table: "EthSwapsETH_TokenEntities",
                column: "blockNumberEndInt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EthSwapsETH_TokenEntities_blockNumberEndInt",
                table: "EthSwapsETH_TokenEntities");

            migrationBuilder.DropColumn(
                name: "blockNumberEndInt",
                table: "EthSwapsETH_TokenEntities");

            migrationBuilder.RenameColumn(
                name: "blockNumberStartInt",
                table: "EthSwapsETH_TokenEntities",
                newName: "blockNumberInt");

            migrationBuilder.RenameIndex(
                name: "IX_EthSwapsETH_TokenEntities_blockNumberStartInt",
                table: "EthSwapsETH_TokenEntities",
                newName: "IX_EthSwapsETH_TokenEntities_blockNumberInt");
        }
    }
}
