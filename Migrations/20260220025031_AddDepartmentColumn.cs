using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRVI.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "HenkatenProblems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 9, 50, 30, 283, DateTimeKind.Local).AddTicks(2321));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 9, 50, 30, 283, DateTimeKind.Local).AddTicks(2325));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 9, 50, 30, 283, DateTimeKind.Local).AddTicks(2327));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 9, 50, 30, 283, DateTimeKind.Local).AddTicks(2337));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 9, 50, 30, 283, DateTimeKind.Local).AddTicks(2338));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 9, 50, 30, 283, DateTimeKind.Local).AddTicks(2338));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 9, 50, 30, 283, DateTimeKind.Local).AddTicks(2348));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 8,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 9, 50, 30, 283, DateTimeKind.Local).AddTicks(2349));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 9,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 9, 50, 30, 283, DateTimeKind.Local).AddTicks(2349));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 10,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 9, 50, 30, 283, DateTimeKind.Local).AddTicks(2359));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 11,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 9, 50, 30, 283, DateTimeKind.Local).AddTicks(2359));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 12,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 9, 50, 30, 283, DateTimeKind.Local).AddTicks(2360));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 13,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 9, 50, 30, 283, DateTimeKind.Local).AddTicks(2368));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 14,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 9, 50, 30, 283, DateTimeKind.Local).AddTicks(2369));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 15,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 20, 9, 50, 30, 283, DateTimeKind.Local).AddTicks(2369));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "HenkatenProblems");

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 16, 14, 56, 58, 449, DateTimeKind.Local).AddTicks(5541));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 16, 14, 56, 58, 449, DateTimeKind.Local).AddTicks(5546));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 16, 14, 56, 58, 449, DateTimeKind.Local).AddTicks(5548));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 16, 14, 56, 58, 449, DateTimeKind.Local).AddTicks(5558));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 16, 14, 56, 58, 449, DateTimeKind.Local).AddTicks(5559));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 16, 14, 56, 58, 449, DateTimeKind.Local).AddTicks(5559));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 16, 14, 56, 58, 449, DateTimeKind.Local).AddTicks(5569));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 8,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 16, 14, 56, 58, 449, DateTimeKind.Local).AddTicks(5570));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 9,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 16, 14, 56, 58, 449, DateTimeKind.Local).AddTicks(5570));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 10,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 16, 14, 56, 58, 449, DateTimeKind.Local).AddTicks(5580));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 11,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 16, 14, 56, 58, 449, DateTimeKind.Local).AddTicks(5580));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 12,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 16, 14, 56, 58, 449, DateTimeKind.Local).AddTicks(5581));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 13,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 16, 14, 56, 58, 449, DateTimeKind.Local).AddTicks(5590));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 14,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 16, 14, 56, 58, 449, DateTimeKind.Local).AddTicks(5590));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 15,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 16, 14, 56, 58, 449, DateTimeKind.Local).AddTicks(5590));
        }
    }
}
