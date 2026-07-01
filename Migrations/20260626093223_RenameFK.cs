using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sports_api.Migrations
{
    /// <inheritdoc />
    public partial class RenameFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Leagues_LeagueID",
                table: "Teams");

            migrationBuilder.RenameColumn(
                name: "LeagueID",
                table: "Teams",
                newName: "LeagueId");

            migrationBuilder.RenameIndex(
                name: "IX_Teams_LeagueID",
                table: "Teams",
                newName: "IX_Teams_LeagueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Leagues_LeagueId",
                table: "Teams",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Leagues_LeagueId",
                table: "Teams");

            migrationBuilder.RenameColumn(
                name: "LeagueId",
                table: "Teams",
                newName: "LeagueID");

            migrationBuilder.RenameIndex(
                name: "IX_Teams_LeagueId",
                table: "Teams",
                newName: "IX_Teams_LeagueID");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Leagues_LeagueID",
                table: "Teams",
                column: "LeagueID",
                principalTable: "Leagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
