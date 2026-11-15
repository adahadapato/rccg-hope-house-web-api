using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCelebrationGalleryImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GalleryImageId",
                table: "Celebrations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Celebrations_GalleryImageId",
                table: "Celebrations",
                column: "GalleryImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Celebrations_GalleryImages_GalleryImageId",
                table: "Celebrations",
                column: "GalleryImageId",
                principalTable: "GalleryImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Celebrations_GalleryImages_GalleryImageId",
                table: "Celebrations");

            migrationBuilder.DropIndex(
                name: "IX_Celebrations_GalleryImageId",
                table: "Celebrations");

            migrationBuilder.DropColumn(
                name: "GalleryImageId",
                table: "Celebrations");
        }
    }
}
