using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AMRVI.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Bagian Produksi Utama", true, "Produksi" },
                    { 2, "Research and Development", true, "Engineering & R&D" },
                    { 3, "Jaminan Mutu Produk", true, "Quality Control & Assurance (QA/QC)" },
                    { 4, "Gudang dan Pengiriman", true, "Logistik & Supply Chain" },
                    { 5, "Sumber Daya Manusia dan Umum", true, "Plant Administration (HR/GA)" },
                    { 6, "Penjualan dan Pemasaran", true, "Sales & Marketing" },
                    { 7, "Perawatan Mesin dan Fasilitas", true, "Maintenance" }
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Departments");

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
    }
}
