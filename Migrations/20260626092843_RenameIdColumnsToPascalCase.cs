using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sports_api.Migrations
{
    /// <inheritdoc />
    public partial class RenameIdColumnsToPascalCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Teams",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Players",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Leagues",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Teams",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Players",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Leagues",
                newName: "ID");
        }
    }
}
