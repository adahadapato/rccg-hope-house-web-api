using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsLocalAndFixChurchServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLocal",
                table: "ChurchServices",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChurchServices_IsLocal",
                table: "ChurchServices",
                column: "IsLocal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChurchServices_IsLocal",
                table: "ChurchServices");

            migrationBuilder.DropColumn(
                name: "IsLocal",
                table: "ChurchServices");
        }
    }
}
