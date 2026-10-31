using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCelebrationInvitationTokenRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "CelebrationInvitationTokens",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "CelebrationInvitationTokens");
        }
    }
}
