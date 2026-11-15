using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RccgHopeHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthlyServiceManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================================================
            // ChurchService monthly-service fields
            // ============================================================

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalInfo",
                table: "ChurchServices",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "ChurchServices",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowInMonthlyServices",
                table: "ChurchServices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // ============================================================
            // Configure the three existing HQ monthly services.
            //
            // ServiceCategory values:
            // ThanksgivingService = 4
            // HolyCommunion        = 9
            // HolyGhostService     = 10
            //
            // IsLocal = 0 prevents the local Thanksgiving Sunday from
            // being treated as the HQ Thanksgiving Service.
            // ============================================================

            migrationBuilder.Sql(
                """
                UPDATE ChurchServices
                SET
                    ShowInMonthlyServices = 1,
                    Icon = N'🍞'
                WHERE IsLocal = 0
                  AND Category = 9;
                """);

            migrationBuilder.Sql(
                """
                UPDATE ChurchServices
                SET
                    ShowInMonthlyServices = 1,
                    Icon = N'🕊️'
                WHERE IsLocal = 0
                  AND Category = 10;
                """);

            migrationBuilder.Sql(
                """
                UPDATE ChurchServices
                SET
                    ShowInMonthlyServices = 1,
                    Icon = N'🙏'
                WHERE IsLocal = 0
                  AND Category = 4;
                """);

            // ============================================================
            // Add ChurchServiceId as nullable first so existing broadcast
            // rows can be backfilled safely.
            // ============================================================

            migrationBuilder.AddColumn<Guid>(
                name: "ChurchServiceId",
                table: "ServiceBroadcasts",
                type: "uniqueidentifier",
                nullable: true);

            // ============================================================
            // Backfill existing broadcasts.
            //
            // Existing broadcasts are matched to the HQ/non-local service
            // having the same ServiceCategory.
            // ============================================================

            migrationBuilder.Sql(
                """
                UPDATE sb
                SET sb.ChurchServiceId = cs.Id
                FROM ServiceBroadcasts AS sb
                INNER JOIN ChurchServices AS cs
                    ON cs.Category = sb.Category
                   AND cs.IsLocal = 0
                WHERE sb.ChurchServiceId IS NULL;
                """);

            // ============================================================
            // Safety check.
            //
            // Abort instead of introducing an invalid relationship if any
            // existing broadcast could not be matched.
            // ============================================================

            migrationBuilder.Sql(
                """
                IF EXISTS
                (
                    SELECT 1
                    FROM ServiceBroadcasts
                    WHERE ChurchServiceId IS NULL
                )
                BEGIN
                    THROW 50001,
                        'Unable to link one or more ServiceBroadcast records to an HQ ChurchService.',
                        1;
                END;
                """);

            // ============================================================
            // Every existing row now has a valid service relationship,
            // so ChurchServiceId can become required.
            // ============================================================

            migrationBuilder.AlterColumn<Guid>(
                name: "ChurchServiceId",
                table: "ServiceBroadcasts",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            // ============================================================
            // Indexes
            // ============================================================

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBroadcasts_ChurchServiceId",
                table: "ServiceBroadcasts",
                column: "ChurchServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBroadcasts_ChurchServiceId_ServiceMonth",
                table: "ServiceBroadcasts",
                columns: new[]
                {
                    "ChurchServiceId",
                    "ServiceMonth"
                },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChurchServices_ShowInMonthlyServices",
                table: "ChurchServices",
                column: "ShowInMonthlyServices");

            // ============================================================
            // Relationship
            // ============================================================

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceBroadcasts_ChurchServices_ChurchServiceId",
                table: "ServiceBroadcasts",
                column: "ChurchServiceId",
                principalTable: "ChurchServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceBroadcasts_ChurchServices_ChurchServiceId",
                table: "ServiceBroadcasts");

            migrationBuilder.DropIndex(
                name: "IX_ServiceBroadcasts_ChurchServiceId",
                table: "ServiceBroadcasts");

            migrationBuilder.DropIndex(
                name: "IX_ServiceBroadcasts_ChurchServiceId_ServiceMonth",
                table: "ServiceBroadcasts");

            migrationBuilder.DropIndex(
                name: "IX_ChurchServices_ShowInMonthlyServices",
                table: "ChurchServices");

            migrationBuilder.DropColumn(
                name: "ChurchServiceId",
                table: "ServiceBroadcasts");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "ChurchServices");

            migrationBuilder.DropColumn(
                name: "ShowInMonthlyServices",
                table: "ChurchServices");

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalInfo",
                table: "ChurchServices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}