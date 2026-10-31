using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCelebrationInvitationTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CelebrationInvitationTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CelebrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenHash = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CelebrationInvitationTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CelebrationInvitationTokens_Celebrations_CelebrationId",
                        column: x => x.CelebrationId,
                        principalTable: "Celebrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CelebrationInvitationTokens_CelebrationId",
                table: "CelebrationInvitationTokens",
                column: "CelebrationId");

            migrationBuilder.CreateIndex(
                name: "IX_CelebrationInvitationTokens_ExpiresAt",
                table: "CelebrationInvitationTokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_CelebrationInvitationTokens_TokenHash",
                table: "CelebrationInvitationTokens",
                column: "TokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CelebrationInvitationTokens");
        }
    }
}
