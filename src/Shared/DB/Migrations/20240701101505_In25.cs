using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.DB.Migrations
{
    /// <inheritdoc />
    public partial class In25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContractSourceCodeTestDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    didXXX = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isGood = table.Column<bool>(type: "bit", nullable: false),
                    typeOfScam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ABI = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContractName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompilerVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptimizationUsed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Runs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConstructorArguments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EVMVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Library = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Proxy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Implementation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SwarmSource = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractSourceCodeTestDatas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractSourceCodeTestDatas");
        }
    }
}
