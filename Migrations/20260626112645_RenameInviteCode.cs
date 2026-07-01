using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sports_api.Migrations
{
    /// <inheritdoc />
    public partial class RenameInviteCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InvitaCode",
                table: "Teams",
                newName: "InviteCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InviteCode",
                table: "Teams",
                newName: "InvitaCode");
        }
    }
}
