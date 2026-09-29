using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDevotionals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devotionals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DevotionalDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Theme = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ScriptureReference = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PassageId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Thought = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CommentaryJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrayerPointsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Declaration = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devotionals", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devotionals_DevotionalDate",
                table: "Devotionals",
                column: "DevotionalDate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devotionals_IsPublished_DevotionalDate",
                table: "Devotionals",
                columns: new[] { "IsPublished", "DevotionalDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devotionals");
        }
    }
}
