using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbMigration.Migrations
{
    /// <inheritdoc />
    public partial class In2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "numberInt",
                table: "slotsEntities",
                type: "decimal(20,0)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "numberInt",
                table: "slotsEntities",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,0)");
        }
    }
}
