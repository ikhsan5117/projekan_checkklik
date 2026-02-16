using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRVI.Migrations
{
    /// <inheritdoc />
    public partial class AddHenkatenAndManPowerTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HenkatenProblems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TanggalUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Shift = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PicLeader = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NamaAreaLine = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NamaOperator = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Jenis4M = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KeteranganProblem = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RencanaPerbaikan = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TanggalRencanaPerbaikan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FotoTemuan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AktualPerbaikan = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TanggalAktualPerbaikan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FotoAktual = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HenkatenProblems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HenkatenProblems_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ManPowers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NIK = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NamaLengkap = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Jabatan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Shift = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AreaLine = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NoTelepon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManPowers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManPowers_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_HenkatenProblems_PlantId",
                table: "HenkatenProblems",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_HenkatenProblems_Status",
                table: "HenkatenProblems",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_HenkatenProblems_TanggalUpdate",
                table: "HenkatenProblems",
                column: "TanggalUpdate");

            migrationBuilder.CreateIndex(
                name: "IX_ManPowers_IsActive",
                table: "ManPowers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ManPowers_NIK",
                table: "ManPowers",
                column: "NIK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManPowers_PlantId",
                table: "ManPowers",
                column: "PlantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HenkatenProblems");

            migrationBuilder.DropTable(
                name: "ManPowers");

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 6, 15, 23, 36, 827, DateTimeKind.Local).AddTicks(5628));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 6, 15, 23, 36, 827, DateTimeKind.Local).AddTicks(5632));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 6, 15, 23, 36, 827, DateTimeKind.Local).AddTicks(5634));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 6, 15, 23, 36, 827, DateTimeKind.Local).AddTicks(5647));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 6, 15, 23, 36, 827, DateTimeKind.Local).AddTicks(5648));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 6, 15, 23, 36, 827, DateTimeKind.Local).AddTicks(5648));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 6, 15, 23, 36, 827, DateTimeKind.Local).AddTicks(5657));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 8,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 6, 15, 23, 36, 827, DateTimeKind.Local).AddTicks(5658));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 9,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 6, 15, 23, 36, 827, DateTimeKind.Local).AddTicks(5658));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 10,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 6, 15, 23, 36, 827, DateTimeKind.Local).AddTicks(5669));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 11,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 6, 15, 23, 36, 827, DateTimeKind.Local).AddTicks(5670));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 12,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 6, 15, 23, 36, 827, DateTimeKind.Local).AddTicks(5670));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 13,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 6, 15, 23, 36, 827, DateTimeKind.Local).AddTicks(5679));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 14,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 6, 15, 23, 36, 827, DateTimeKind.Local).AddTicks(5679));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 15,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 6, 15, 23, 36, 827, DateTimeKind.Local).AddTicks(5680));
        }
    }
}
