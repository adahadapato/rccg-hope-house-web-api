using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBirthdayWishes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GalleryImageComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GalleryImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DecisionSource = table.Column<int>(type: "int", nullable: false),
                    DecisionReason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ModeratedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ModeratedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryImageComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GalleryImageComments_GalleryImages_GalleryImageId",
                        column: x => x.GalleryImageId,
                        principalTable: "GalleryImages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GalleryImageComments_GalleryImageId_Status",
                table: "GalleryImageComments",
                columns: new[] { "GalleryImageId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_GalleryImageComments_Status_CreatedAt",
                table: "GalleryImageComments",
                columns: new[] { "Status", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GalleryImageComments");
        }
    }
}
