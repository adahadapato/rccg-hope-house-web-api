using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChurchServiceBroadcastEnabled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBroadcastEnabled",
                table: "ChurchServices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ChurchServices_IsBroadcastEnabled",
                table: "ChurchServices",
                column: "IsBroadcastEnabled");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChurchServices_IsBroadcastEnabled",
                table: "ChurchServices");

            migrationBuilder.DropColumn(
                name: "IsBroadcastEnabled",
                table: "ChurchServices");
        }
    }
}
