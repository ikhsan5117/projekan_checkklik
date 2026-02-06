using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AMRVI.Migrations
{
    /// <inheritdoc />
    public partial class CreateAndonDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlantId",
                table: "Machines_RVI",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FourMCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ColorCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FourMCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlantName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ColorCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AndonMachines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantId = table.Column<int>(type: "int", nullable: false),
                    MachineCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MachineName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AndonMachines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AndonMachines_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AndonRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantId = table.Column<int>(type: "int", nullable: false),
                    MachineId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    FourMCategoryId = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AndonRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AndonRecords_AndonMachines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "AndonMachines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AndonRecords_FourMCategories_FourMCategoryId",
                        column: x => x.FourMCategoryId,
                        principalTable: "FourMCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AndonRecords_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AndonRecords_StatusTypes_StatusId",
                        column: x => x.StatusId,
                        principalTable: "StatusTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "FourMCategories",
                columns: new[] { "Id", "CategoryCode", "CategoryName", "ColorCode", "Priority" },
                values: new object[,]
                {
                    { 1, "MACHINE", "MACHINE", "#ec4899", 1 },
                    { 2, "MATERIAL", "MATERIAL", "#eab308", 2 },
                    { 3, "MAN", "MAN", "#22d3ee", 3 },
                    { 4, "METHODE", "METHODE", "#a855f7", 4 },
                    { 5, "NO_PROBLEM", "NO PROBLEM", "#6b7280", 5 }
                });

            migrationBuilder.UpdateData(
                table: "Machines_RVI",
                keyColumn: "Id",
                keyValue: 1,
                column: "PlantId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Machines_RVI",
                keyColumn: "Id",
                keyValue: 2,
                column: "PlantId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Machines_RVI",
                keyColumn: "Id",
                keyValue: 3,
                column: "PlantId",
                value: null);

            migrationBuilder.InsertData(
                table: "Plants",
                columns: new[] { "Id", "CreatedAt", "IsActive", "PlantCode", "PlantName" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "RVI", "Rubber Vibration Isolator" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "BTR", "Bridgestone" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "HOSE", "HOSE Production" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "MOLDED", "MOLDED Production" },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "MIXING", "MIXING Production" }
                });

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

            migrationBuilder.InsertData(
                table: "StatusTypes",
                columns: new[] { "Id", "ColorCode", "Priority", "StatusCode", "StatusName" },
                values: new object[,]
                {
                    { 1, "#ef4444", 1, "LINE_STOP", "LINE STOP" },
                    { 2, "#3b82f6", 2, "NO_LOADING", "NO LOADING" },
                    { 3, "#f97316", 3, "NO_RUNNING", "NO RUNNING" },
                    { 4, "#10b981", 4, "RUNNING", "RUNNING" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Machines_RVI_PlantId",
                table: "Machines_RVI",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_AndonMachines_PlantId_MachineCode",
                table: "AndonMachines",
                columns: new[] { "PlantId", "MachineCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AndonRecords_FourMCategoryId",
                table: "AndonRecords",
                column: "FourMCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AndonRecords_MachineId",
                table: "AndonRecords",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_AndonRecords_PlantId",
                table: "AndonRecords",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_AndonRecords_RecordedAt",
                table: "AndonRecords",
                column: "RecordedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AndonRecords_StatusId",
                table: "AndonRecords",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_FourMCategories_CategoryCode",
                table: "FourMCategories",
                column: "CategoryCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plants_PlantCode",
                table: "Plants",
                column: "PlantCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatusTypes_StatusCode",
                table: "StatusTypes",
                column: "StatusCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Machines_RVI_Plants_PlantId",
                table: "Machines_RVI",
                column: "PlantId",
                principalTable: "Plants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Machines_RVI_Plants_PlantId",
                table: "Machines_RVI");

            migrationBuilder.DropTable(
                name: "AndonRecords");

            migrationBuilder.DropTable(
                name: "AndonMachines");

            migrationBuilder.DropTable(
                name: "FourMCategories");

            migrationBuilder.DropTable(
                name: "StatusTypes");

            migrationBuilder.DropTable(
                name: "Plants");

            migrationBuilder.DropIndex(
                name: "IX_Machines_RVI_PlantId",
                table: "Machines_RVI");

            migrationBuilder.DropColumn(
                name: "PlantId",
                table: "Machines_RVI");

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1815));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1820));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1821));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1832));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1832));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1833));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1843));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 8,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1843));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 9,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1843));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 10,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1872));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 11,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1872));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 12,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1873));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 13,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1882));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 14,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1883));

            migrationBuilder.UpdateData(
                table: "ShiftSettings",
                keyColumn: "Id",
                keyValue: 15,
                column: "UpdatedAt",
                value: new DateTime(2026, 2, 5, 10, 1, 10, 333, DateTimeKind.Local).AddTicks(1884));
        }
    }
}
