using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAnnouncementEventDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EventEndsAt",
                table: "Announcements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EventStartsAt",
                table: "Announcements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VenueAddress",
                table: "Announcements",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VenueName",
                table: "Announcements",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventEndsAt",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "EventStartsAt",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "VenueAddress",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "VenueName",
                table: "Announcements");
        }
    }
}
