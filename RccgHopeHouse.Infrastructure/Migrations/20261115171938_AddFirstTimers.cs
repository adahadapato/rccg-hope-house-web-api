using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFirstTimers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FirstTimers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NormalizedPhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AddressLine1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Postcode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BirthMonth = table.Column<int>(type: "int", nullable: true),
                    BirthDay = table.Column<int>(type: "int", nullable: true),
                    Source = table.Column<int>(type: "int", nullable: true),
                    InvitedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    WantsPastoralContact = table.Column<bool>(type: "bit", nullable: false),
                    PreferredContactMethod = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ConsentToContact = table.Column<bool>(type: "bit", nullable: false),
                    FirstVisitDate = table.Column<DateOnly>(type: "date", nullable: false),
                    LastVisitDate = table.Column<DateOnly>(type: "date", nullable: false),
                    VisitCount = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    FollowUpStatus = table.Column<int>(type: "int", nullable: false),
                    FollowUpNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PrayerRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PromotedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FirstTimers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FirstTimers_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FirstTimers_PrayerRequests_PrayerRequestId",
                        column: x => x.PrayerRequestId,
                        principalTable: "PrayerRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FirstTimers_FollowUpStatus",
                table: "FirstTimers",
                column: "FollowUpStatus");

            migrationBuilder.CreateIndex(
                name: "IX_FirstTimers_LastVisitDate",
                table: "FirstTimers",
                column: "LastVisitDate");

            migrationBuilder.CreateIndex(
                name: "IX_FirstTimers_MemberId",
                table: "FirstTimers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_FirstTimers_NormalizedPhoneNumber",
                table: "FirstTimers",
                column: "NormalizedPhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_FirstTimers_PrayerRequestId",
                table: "FirstTimers",
                column: "PrayerRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FirstTimers");
        }
    }
}
