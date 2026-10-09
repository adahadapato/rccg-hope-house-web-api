using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDevotionalAdditionalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalReading",
                table: "Devotionals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Devotionals",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BibleInOneYearPassageIdsJson",
                table: "Devotionals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "BibleInOneYearReference",
                table: "Devotionals",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HymnLyrics",
                table: "Devotionals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HymnNumber",
                table: "Devotionals",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HymnTitle",
                table: "Devotionals",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "KeyPoint",
                table: "Devotionals",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MemoryVersePassageId",
                table: "Devotionals",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MemoryVerseReference",
                table: "Devotionals",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SourceUrl",
                table: "Devotionals",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalReading",
                table: "Devotionals");

            migrationBuilder.DropColumn(
                name: "Author",
                table: "Devotionals");

            migrationBuilder.DropColumn(
                name: "BibleInOneYearPassageIdsJson",
                table: "Devotionals");

            migrationBuilder.DropColumn(
                name: "BibleInOneYearReference",
                table: "Devotionals");

            migrationBuilder.DropColumn(
                name: "HymnLyrics",
                table: "Devotionals");

            migrationBuilder.DropColumn(
                name: "HymnNumber",
                table: "Devotionals");

            migrationBuilder.DropColumn(
                name: "HymnTitle",
                table: "Devotionals");

            migrationBuilder.DropColumn(
                name: "KeyPoint",
                table: "Devotionals");

            migrationBuilder.DropColumn(
                name: "MemoryVersePassageId",
                table: "Devotionals");

            migrationBuilder.DropColumn(
                name: "MemoryVerseReference",
                table: "Devotionals");

            migrationBuilder.DropColumn(
                name: "SourceUrl",
                table: "Devotionals");
        }
    }
}
