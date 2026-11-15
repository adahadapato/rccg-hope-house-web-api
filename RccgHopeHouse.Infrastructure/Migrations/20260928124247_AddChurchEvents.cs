using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddChurchEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChurchEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RegistrationUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RegistrationButtonText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "Register Now"),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChurchEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChurchEvents_Category",
                table: "ChurchEvents",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_ChurchEvents_DisplayOrder",
                table: "ChurchEvents",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ChurchEvents_EndDateTime",
                table: "ChurchEvents",
                column: "EndDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_ChurchEvents_IsActive",
                table: "ChurchEvents",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ChurchEvents_IsActive_StartDateTime",
                table: "ChurchEvents",
                columns: new[] { "IsActive", "StartDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_ChurchEvents_StartDateTime",
                table: "ChurchEvents",
                column: "StartDateTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChurchEvents");
        }
    }
}
