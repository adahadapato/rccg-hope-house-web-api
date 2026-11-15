using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeoMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SeoMetadata",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ContentTitle = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SeoTitle = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    MetaDescription = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Keywords = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeoMetadata", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeoMetadata_ContentTitle",
                table: "SeoMetadata",
                column: "ContentTitle");

            migrationBuilder.CreateIndex(
                name: "IX_SeoMetadata_ContentType_ContentId",
                table: "SeoMetadata",
                columns: new[] { "ContentType", "ContentId" },
                unique: true,
                filter: "[ContentId] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SeoMetadata_ContentType_IsActive",
                table: "SeoMetadata",
                columns: new[] { "ContentType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_SeoMetadata_PagePath",
                table: "SeoMetadata",
                column: "PagePath",
                unique: true,
                filter: "[PagePath] IS NOT NULL AND [IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeoMetadata");
        }
    }
}
