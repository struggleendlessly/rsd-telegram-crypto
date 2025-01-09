using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbMigration.Migrations
{
    /// <inheritdoc />
    public partial class In8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "t0amount",
                table: "swapsTokensUSDEntities");

            migrationBuilder.DropColumn(
                name: "t1amount",
                table: "swapsTokensUSDEntities");

            migrationBuilder.RenameColumn(
                name: "t1amount",
                table: "swapsTokensEntities",
                newName: "tokenOut");

            migrationBuilder.RenameColumn(
                name: "t0amount",
                table: "swapsTokensEntities",
                newName: "tokenIn");

            migrationBuilder.AddColumn<double>(
                name: "solIn",
                table: "swapsTokensEntities",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "solOut",
                table: "swapsTokensEntities",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "solIn",
                table: "swapsTokensEntities");

            migrationBuilder.DropColumn(
                name: "solOut",
                table: "swapsTokensEntities");

            migrationBuilder.RenameColumn(
                name: "tokenOut",
                table: "swapsTokensEntities",
                newName: "t1amount");

            migrationBuilder.RenameColumn(
                name: "tokenIn",
                table: "swapsTokensEntities",
                newName: "t0amount");

            migrationBuilder.AddColumn<double>(
                name: "t0amount",
                table: "swapsTokensUSDEntities",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "t1amount",
                table: "swapsTokensUSDEntities",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
