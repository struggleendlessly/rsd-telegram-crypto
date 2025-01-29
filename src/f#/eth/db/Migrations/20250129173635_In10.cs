using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace db.Migrations
{
    /// <inheritdoc />
    public partial class In10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "priceTokenInETH",
                table: "swapsETH_TokenEntities",
                type: "decimal(38,20)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,20)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "priceTokenInETH",
                table: "swapsETH_TokenEntities",
                type: "decimal(28,20)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38,20)");
        }
    }
}
