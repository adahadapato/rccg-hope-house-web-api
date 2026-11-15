using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorGalleryToFileStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "GalleryImages");

            migrationBuilder.DropColumn(
                name: "ThumbnailData",
                table: "GalleryImages");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "GalleryImages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "GalleryImages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailPath",
                table: "GalleryImages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GalleryImages_DisplayOrder",
                table: "GalleryImages",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryImages_ImageHash",
                table: "GalleryImages",
                column: "ImageHash");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryImages_IsPublic",
                table: "GalleryImages",
                column: "IsPublic");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GalleryImages_DisplayOrder",
                table: "GalleryImages");

            migrationBuilder.DropIndex(
                name: "IX_GalleryImages_ImageHash",
                table: "GalleryImages");

            migrationBuilder.DropIndex(
                name: "IX_GalleryImages_IsPublic",
                table: "GalleryImages");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "GalleryImages");

            migrationBuilder.DropColumn(
                name: "ThumbnailPath",
                table: "GalleryImages");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "GalleryImages",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "GalleryImages",
                type: "VARBINARY(MAX)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "ThumbnailData",
                table: "GalleryImages",
                type: "VARBINARY(MAX)",
                nullable: true);
        }
    }
}
