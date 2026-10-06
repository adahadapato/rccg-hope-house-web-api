using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberBirthdayPublicationAndPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConsentToBirthdayPublication",
                table: "Members",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PhotoId",
                table: "Members",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_ConsentToBirthdayPublication",
                table: "Members",
                column: "ConsentToBirthdayPublication");

            migrationBuilder.CreateIndex(
                name: "IX_Members_PhotoId",
                table: "Members",
                column: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_GalleryImages_PhotoId",
                table: "Members",
                column: "PhotoId",
                principalTable: "GalleryImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_GalleryImages_PhotoId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_ConsentToBirthdayPublication",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_PhotoId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ConsentToBirthdayPublication",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Members");
        }
    }
}
