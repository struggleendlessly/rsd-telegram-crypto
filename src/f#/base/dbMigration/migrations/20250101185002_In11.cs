using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dbMigration.migrations
{
    /// <inheritdoc />
    public partial class In11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "EthTokenInfoEntities",
                newName: "AddressToken");

            migrationBuilder.RenameIndex(
                name: "IX_EthTokenInfoEntities_Address",
                table: "EthTokenInfoEntities",
                newName: "IX_EthTokenInfoEntities_AddressToken");

            migrationBuilder.AddColumn<string>(
                name: "AddressPair",
                table: "EthTokenInfoEntities",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_EthTokenInfoEntities_AddressPair",
                table: "EthTokenInfoEntities",
                column: "AddressPair");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EthTokenInfoEntities_AddressPair",
                table: "EthTokenInfoEntities");

            migrationBuilder.DropColumn(
                name: "AddressPair",
                table: "EthTokenInfoEntities");

            migrationBuilder.RenameColumn(
                name: "AddressToken",
                table: "EthTokenInfoEntities",
                newName: "Address");

            migrationBuilder.RenameIndex(
                name: "IX_EthTokenInfoEntities_AddressToken",
                table: "EthTokenInfoEntities",
                newName: "IX_EthTokenInfoEntities_Address");
        }
    }
}
