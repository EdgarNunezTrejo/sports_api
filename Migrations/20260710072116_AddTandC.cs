using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sports_api.Migrations
{
    /// <inheritdoc />
    public partial class AddTandC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AcceptsTermsAndConditions",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptsTermsAndConditions",
                table: "Users");
        }
    }
}
