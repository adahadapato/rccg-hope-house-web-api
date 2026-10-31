using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCelebrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Celebrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GalleryCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Occasion = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CelebrationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    PhotoPublicationConsent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    InvitationSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedByAdminAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WithdrawnAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Celebrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Celebrations_GalleryCategories_GalleryCategoryId",
                        column: x => x.GalleryCategoryId,
                        principalTable: "GalleryCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Celebrations_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Celebrations_Date_Status",
                table: "Celebrations",
                columns: new[] { "CelebrationDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Celebrations_GalleryCategoryId",
                table: "Celebrations",
                column: "GalleryCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Celebrations_Member_Date",
                table: "Celebrations",
                columns: new[] { "MemberId", "CelebrationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Celebrations_Member_Occasion_Date",
                table: "Celebrations",
                columns: new[] { "MemberId", "Occasion", "CelebrationDate" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Celebrations");
        }
    }
}
