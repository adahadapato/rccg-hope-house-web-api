using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAnnualPrayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnnualPrayer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Theme = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Service = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BibleReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BibleText = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Declaration = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ClosingVerse = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualPrayer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnnualPrayerPoint",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnnualPrayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualPrayerPoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnualPrayerPoint_AnnualPrayer_AnnualPrayerId",
                        column: x => x.AnnualPrayerId,
                        principalTable: "AnnualPrayer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnnualPrayer_Year",
                table: "AnnualPrayer",
                column: "Year",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnnualPrayerPoint_AnnualPrayerId",
                table: "AnnualPrayerPoint",
                column: "AnnualPrayerId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnualPrayerPoint_AnnualPrayerId_DisplayOrder",
                table: "AnnualPrayerPoint",
                columns: new[] { "AnnualPrayerId", "DisplayOrder" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnualPrayerPoint");

            migrationBuilder.DropTable(
                name: "AnnualPrayer");
        }
    }
}
