using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AMRVI.Migrations
{
    /// <inheritdoc />
    public partial class AddShiftSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShiftSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plant = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShiftNumber = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ShiftSettings",
                columns: new[] { "Id", "EndTime", "Plant", "ShiftNumber", "StartTime", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new TimeSpan(0, 14, 0, 0, 0), "RVI", 1, new TimeSpan(0, 5, 0, 0, 0), new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1815) },
                    { 2, new TimeSpan(0, 22, 0, 0, 0), "RVI", 2, new TimeSpan(0, 14, 0, 0, 0), new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1820) },
                    { 3, new TimeSpan(0, 5, 0, 0, 0), "RVI", 3, new TimeSpan(0, 22, 0, 0, 0), new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1821) },
                    { 4, new TimeSpan(0, 14, 0, 0, 0), "BTR", 1, new TimeSpan(0, 5, 0, 0, 0), new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1832) },
                    { 5, new TimeSpan(0, 22, 0, 0, 0), "BTR", 2, new TimeSpan(0, 14, 0, 0, 0), new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1832) },
                    { 6, new TimeSpan(0, 5, 0, 0, 0), "BTR", 3, new TimeSpan(0, 22, 0, 0, 0), new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1833) },
                    { 7, new TimeSpan(0, 14, 0, 0, 0), "HOSE", 1, new TimeSpan(0, 5, 0, 0, 0), new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1843) },
                    { 8, new TimeSpan(0, 22, 0, 0, 0), "HOSE", 2, new TimeSpan(0, 14, 0, 0, 0), new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1843) },
                    { 9, new TimeSpan(0, 5, 0, 0, 0), "HOSE", 3, new TimeSpan(0, 22, 0, 0, 0), new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1843) },
                    { 10, new TimeSpan(0, 14, 0, 0, 0), "MOLDED", 1, new TimeSpan(0, 5, 0, 0, 0), new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1872) },
                    { 11, new TimeSpan(0, 22, 0, 0, 0), "MOLDED", 2, new TimeSpan(0, 14, 0, 0, 0), new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1872) },
                    { 12, new TimeSpan(0, 5, 0, 0, 0), "MOLDED", 3, new TimeSpan(0, 22, 0, 0, 0), new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1873) },
                    { 13, new TimeSpan(0, 14, 0, 0, 0), "MIXING", 1, new TimeSpan(0, 5, 0, 0, 0), new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1882) },
                    { 14, new TimeSpan(0, 22, 0, 0, 0), "MIXING", 2, new TimeSpan(0, 14, 0, 0, 0), new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1883) },
                    { 15, new TimeSpan(0, 5, 0, 0, 0), "MIXING", 3, new TimeSpan(0, 22, 0, 0, 0), new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1884) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShiftSettings");
        }
    }
}
