using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apiWebBrowserParser.migrations
{
    /// <inheritdoc />
    public partial class AddApplicationActivationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserKey",
                table: "messagesEntities",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ApplicationActivations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ActivatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationActivations", x => x.Id);
                    table.UniqueConstraint("AK_ApplicationActivations_UserKey", x => x.UserKey);
                });

            migrationBuilder.CreateIndex(
                name: "IX_messagesEntities_UserKey",
                table: "messagesEntities",
                column: "UserKey");

            migrationBuilder.AddForeignKey(
                name: "FK_messagesEntities_ApplicationActivations_UserKey",
                table: "messagesEntities",
                column: "UserKey",
                principalTable: "ApplicationActivations",
                principalColumn: "UserKey",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messagesEntities_ApplicationActivations_UserKey",
                table: "messagesEntities");

            migrationBuilder.DropTable(
                name: "ApplicationActivations");

            migrationBuilder.DropIndex(
                name: "IX_messagesEntities_UserKey",
                table: "messagesEntities");

            migrationBuilder.DropColumn(
                name: "UserKey",
                table: "messagesEntities");
        }
    }
}
