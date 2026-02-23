using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRVI.Migrations
{
    /// <inheritdoc />
    public partial class AddStandard4MAndTemporaryActionToHenkaten : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Actual4M",
                table: "HenkatenProblems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Standard4M",
                table: "HenkatenProblems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemporaryAction",
                table: "HenkatenProblems",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 23, 13, 23, 49, 248, DateTimeKind.Local).AddTicks(5882));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 23, 13, 23, 49, 248, DateTimeKind.Local).AddTicks(5887));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 23, 13, 23, 49, 248, DateTimeKind.Local).AddTicks(5888));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 23, 13, 23, 49, 248, DateTimeKind.Local).AddTicks(5900));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 23, 13, 23, 49, 248, DateTimeKind.Local).AddTicks(5900));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 23, 13, 23, 49, 248, DateTimeKind.Local).AddTicks(5901));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 23, 13, 23, 49, 248, DateTimeKind.Local).AddTicks(5913));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 8,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 23, 13, 23, 49, 248, DateTimeKind.Local).AddTicks(5913));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 9,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 23, 13, 23, 49, 248, DateTimeKind.Local).AddTicks(5914));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 10,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 23, 13, 23, 49, 248, DateTimeKind.Local).AddTicks(5925));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 11,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 23, 13, 23, 49, 248, DateTimeKind.Local).AddTicks(5925));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 12,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 23, 13, 23, 49, 248, DateTimeKind.Local).AddTicks(5926));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 13,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 23, 13, 23, 49, 248, DateTimeKind.Local).AddTicks(5936));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 14,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 23, 13, 23, 49, 248, DateTimeKind.Local).AddTicks(5937));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 15,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 23, 13, 23, 49, 248, DateTimeKind.Local).AddTicks(5937));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Actual4M",
                table: "HenkatenProblems");

            migrationBuilder.DropColumn(
                name: "Standard4M",
                table: "HenkatenProblems");

            migrationBuilder.DropColumn(
                name: "TemporaryAction",
                table: "HenkatenProblems");

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 10, 6, 26, 837, DateTimeKind.Local).AddTicks(5929));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 10, 6, 26, 837, DateTimeKind.Local).AddTicks(5935));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 10, 6, 26, 837, DateTimeKind.Local).AddTicks(5936));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 10, 6, 26, 837, DateTimeKind.Local).AddTicks(5950));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 10, 6, 26, 837, DateTimeKind.Local).AddTicks(5951));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 10, 6, 26, 837, DateTimeKind.Local).AddTicks(5951));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 10, 6, 26, 837, DateTimeKind.Local).AddTicks(5963));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 8,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 10, 6, 26, 837, DateTimeKind.Local).AddTicks(5964));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 9,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 10, 6, 26, 837, DateTimeKind.Local).AddTicks(5964));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 10,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 10, 6, 26, 837, DateTimeKind.Local).AddTicks(5976));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 11,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 10, 6, 26, 837, DateTimeKind.Local).AddTicks(5976));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 12,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 10, 6, 26, 837, DateTimeKind.Local).AddTicks(5977));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 13,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 10, 6, 26, 837, DateTimeKind.Local).AddTicks(5986));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 14,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 10, 6, 26, 837, DateTimeKind.Local).AddTicks(5987));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 15,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 10, 6, 26, 837, DateTimeKind.Local).AddTicks(5987));
        }
    }
}
