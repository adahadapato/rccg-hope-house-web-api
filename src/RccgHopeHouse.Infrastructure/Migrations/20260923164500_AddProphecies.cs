using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProphecies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProphecyYears",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProphecyYears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProphecyCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ProphecyYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProphecyCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProphecyCategories_ProphecyYears_ProphecyYearId",
                        column: x => x.ProphecyYearId,
                        principalTable: "ProphecyYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prophecies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prophecies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prophecies_ProphecyCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProphecyCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prophecies_CategoryId",
                table: "Prophecies",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Prophecies_DisplayOrder",
                table: "Prophecies",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Prophecies_IsActive",
                table: "Prophecies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ProphecyCategories_DisplayOrder",
                table: "ProphecyCategories",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ProphecyCategories_IsActive",
                table: "ProphecyCategories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ProphecyCategories_ProphecyYearId",
                table: "ProphecyCategories",
                column: "ProphecyYearId");

            migrationBuilder.CreateIndex(
                name: "IX_ProphecyCategories_ProphecyYearId_Name",
                table: "ProphecyCategories",
                columns: new[] { "ProphecyYearId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProphecyYears_IsPublished",
                table: "ProphecyYears",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_ProphecyYears_Year",
                table: "ProphecyYears",
                column: "Year",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prophecies");

            migrationBuilder.DropTable(
                name: "ProphecyCategories");

            migrationBuilder.DropTable(
                name: "ProphecyYears");
        }
    }
}
