using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AMRVI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineId = table.Column<int>(type: "int", nullable: false),
                    OrderNumber = table.Column<int>(type: "int", nullable: false),
                    DetailName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StandardDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistItems_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MachineNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineId = table.Column<int>(type: "int", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MachineNumbers_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineNumberId = table.Column<int>(type: "int", nullable: false),
                    InspectorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionSessions_MachineNumbers_MachineNumberId",
                        column: x => x.MachineNumberId,
                        principalTable: "MachineNumbers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InspectionSessionId = table.Column<int>(type: "int", nullable: false),
                    ChecklistItemId = table.Column<int>(type: "int", nullable: false),
                    Judgement = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionResults_ChecklistItems_ChecklistItemId",
                        column: x => x.ChecklistItemId,
                        principalTable: "ChecklistItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectionResults_InspectionSessions_InspectionSessionId",
                        column: x => x.InspectionSessionId,
                        principalTable: "InspectionSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Machines",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(4873), "Injection Molding Machine", true, "MESIN INJECTION" },
                    { 2, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(4875), "Press Machine", true, "MESIN PRESS" },
                    { 3, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(4878), "CNC Machine", true, "MESIN CNC" }
                });

            migrationBuilder.InsertData(
                table: "ChecklistItems",
                columns: new[] { "Id", "CreatedAt", "DetailName", "ImagePath", "IsActive", "MachineId", "OrderNumber", "StandardDescription" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5064), "Emergency Stop", "/images/checklist/injection/emergency-stop.jpg", true, 1, 1, "Pastikan tombol emergency stop berfungsi dengan baik dan mudah dijangkau" },
                    { 2, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5067), "Safety Light Curtain", "/images/checklist/injection/safety-light-curtain.jpg", true, 1, 2, "Pastikan pergerakan Ejector & Mold berhenti saat light curtain aktif" },
                    { 3, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5069), "Safety Door", "/images/checklist/injection/safety-door.jpg", true, 1, 3, "Pastikan pintu safety tertutup rapat dan interlock berfungsi" },
                    { 4, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5071), "Hydraulic Pressure", "/images/checklist/injection/hydraulic-pressure.jpg", true, 1, 4, "Cek tekanan hydraulic dalam range normal (150-200 bar)" },
                    { 5, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5073), "Heating System", "/images/checklist/injection/heating-system.jpg", true, 1, 5, "Pastikan suhu barrel sesuai dengan setting point" },
                    { 6, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5075), "Cooling System", "/images/checklist/injection/cooling-system.jpg", true, 1, 6, "Cek aliran air cooling dan suhu" },
                    { 7, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5077), "Mold Condition", "/images/checklist/injection/mold-condition.jpg", true, 1, 7, "Pastikan mold bersih dan tidak ada kerusakan" },
                    { 8, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5079), "Ejector System", "/images/checklist/injection/ejector-system.jpg", true, 1, 8, "Cek pergerakan ejector smooth dan tidak macet" },
                    { 9, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5081), "Hopper Dryer", "/images/checklist/injection/hopper-dryer.jpg", true, 1, 9, "Pastikan material kering dan suhu dryer sesuai" },
                    { 10, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5110), "Robot Arm", "/images/checklist/injection/robot-arm.jpg", true, 1, 10, "Cek pergerakan robot arm dan vacuum system" },
                    { 11, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5112), "Oil Level", "/images/checklist/injection/oil-level.jpg", true, 1, 11, "Pastikan level oli hydraulic dalam batas normal" },
                    { 12, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5114), "Cleanliness", "/images/checklist/injection/cleanliness.jpg", true, 1, 12, "Area mesin bersih dari oli, material, dan kotoran" },
                    { 13, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5117), "Control Panel", "/images/checklist/injection/control-panel.jpg", true, 1, 13, "Pastikan display dan tombol kontrol berfungsi normal" }
                });

            migrationBuilder.InsertData(
                table: "MachineNumbers",
                columns: new[] { "Id", "CreatedAt", "IsActive", "Location", "MachineId", "Number" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5032), true, "Area A", 1, "1" },
                    { 2, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5034), true, "Area A", 1, "2" },
                    { 3, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5036), true, "Area B", 1, "3" },
                    { 4, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5038), true, "Area C", 2, "1" },
                    { 5, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5040), true, "Area C", 2, "2" },
                    { 6, new DateTime(2026, 1, 30, 10, 22, 58, 492, DateTimeKind.Local).AddTicks(5042), true, "Area D", 3, "1" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItems_MachineId_OrderNumber",
                table: "ChecklistItems",
                columns: new[] { "MachineId", "OrderNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionResults_ChecklistItemId",
                table: "InspectionResults",
                column: "ChecklistItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionResults_InspectionSessionId",
                table: "InspectionResults",
                column: "InspectionSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionSessions_InspectionDate",
                table: "InspectionSessions",
                column: "InspectionDate");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionSessions_MachineNumberId",
                table: "InspectionSessions",
                column: "MachineNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_MachineNumbers_MachineId_Number",
                table: "MachineNumbers",
                columns: new[] { "MachineId", "Number" });

            migrationBuilder.CreateIndex(
                name: "IX_Machines_Name",
                table: "Machines",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InspectionResults");

            migrationBuilder.DropTable(
                name: "ChecklistItems");

            migrationBuilder.DropTable(
                name: "InspectionSessions");

            migrationBuilder.DropTable(
                name: "MachineNumbers");

            migrationBuilder.DropTable(
                name: "Machines");
        }
    }
}
