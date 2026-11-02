using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAiPrompts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AiPromptVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PromptKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeNote = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ChangedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiPromptVersions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AiPromptVersions_PromptKey_IsActive",
                table: "AiPromptVersions",
                columns: new[] { "PromptKey", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_AiPromptVersions_PromptKey_Version",
                table: "AiPromptVersions",
                columns: new[] { "PromptKey", "Version" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiPromptVersions");
        }
    }
}
