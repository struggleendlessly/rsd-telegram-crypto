using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbMigration.Migrations
{
    /// <inheritdoc />
    public partial class In6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "solIn",
                table: "swapsTokensEntities");

            migrationBuilder.DropColumn(
                name: "solOut",
                table: "swapsTokensEntities");

            migrationBuilder.DropColumn(
                name: "tokenIn",
                table: "swapsTokensEntities");

            migrationBuilder.DropColumn(
                name: "tokenOut",
                table: "swapsTokensEntities");

            migrationBuilder.AddColumn<double>(
                name: "t0amount",
                table: "swapsTokensEntities",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "t1amount",
                table: "swapsTokensEntities",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "t0amount",
                table: "swapsTokensEntities");

            migrationBuilder.DropColumn(
                name: "t1amount",
                table: "swapsTokensEntities");

            migrationBuilder.AddColumn<string>(
                name: "solIn",
                table: "swapsTokensEntities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "solOut",
                table: "swapsTokensEntities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "tokenIn",
                table: "swapsTokensEntities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "tokenOut",
                table: "swapsTokensEntities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
