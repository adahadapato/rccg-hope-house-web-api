using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteVisits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteVisits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitDate = table.Column<DateOnly>(type: "date", nullable: false),
                    VisitorHash = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                    IsMobile = table.Column<bool>(type: "bit", nullable: false),
                    PageViews = table.Column<int>(type: "int", nullable: false),
                    EngagedSeconds = table.Column<int>(type: "int", nullable: false),
                    FirstSeenAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteVisits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SiteVisits_LastSeenAt",
                table: "SiteVisits",
                column: "LastSeenAt");

            migrationBuilder.CreateIndex(
                name: "IX_SiteVisits_VisitDate_VisitorHash",
                table: "SiteVisits",
                columns: new[] { "VisitDate", "VisitorHash" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteVisits");
        }
    }
}
