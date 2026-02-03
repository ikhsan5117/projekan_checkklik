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
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
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
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Injection Molding Machine", true, "MESIN INJECTION" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Press Machine", true, "MESIN PRESS" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CNC Machine", true, "MESIN CNC" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Department", "Email", "FullName", "IsActive", "LastLogin", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "IT", "admin@amrvi.com", "Administrator", true, null, "$2a$11$Xovj8PUyRtOHoVuMP7ZQg.esnQcImj2if8ZRScrgpa89IdNL7Rm5S", "Admin", "admin" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Production", "supervisor@amrvi.com", "Supervisor Production", true, null, "$2a$11$.Ns3ftRfJyJuPGQrzWhVReFhSnMDE4CEN2ToE/meiMQtv6pNN3wgO", "Supervisor", "supervisor" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Production", "operator1@amrvi.com", "Operator Mesin 1", true, null, "$2a$11$nEek27hakB6XYX8YlT7FP.8r9hQ9Ww1ouHAAHD84RrDitYDPwASdK", "User", "operator1" }
                });

            migrationBuilder.InsertData(
                table: "ChecklistItems",
                columns: new[] { "Id", "CreatedAt", "DetailName", "ImagePath", "IsActive", "MachineId", "OrderNumber", "StandardDescription" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Emergency Stop", "/images/checklist/injection/emergency-stop.jpg", true, 1, 1, "Pastikan tombol emergency stop berfungsi dengan baik dan mudah dijangkau" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Safety Light Curtain", "/images/checklist/injection/safety-light-curtain.jpg", true, 1, 2, "Pastikan pergerakan Ejector & Mold berhenti saat light curtain aktif" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Safety Door", "/images/checklist/injection/safety-door.jpg", true, 1, 3, "Pastikan pintu safety tertutup rapat dan interlock berfungsi" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hydraulic Pressure", "/images/checklist/injection/hydraulic-pressure.jpg", true, 1, 4, "Cek tekanan hydraulic dalam range normal (150-200 bar)" },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Heating System", "/images/checklist/injection/heating-system.jpg", true, 1, 5, "Pastikan suhu barrel sesuai dengan setting point" },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cooling System", "/images/checklist/injection/cooling-system.jpg", true, 1, 6, "Cek aliran air cooling dan suhu" },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mold Condition", "/images/checklist/injection/mold-condition.jpg", true, 1, 7, "Pastikan mold bersih dan tidak ada kerusakan" },
                    { 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ejector System", "/images/checklist/injection/ejector-system.jpg", true, 1, 8, "Cek pergerakan ejector smooth dan tidak macet" },
                    { 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hopper Dryer", "/images/checklist/injection/hopper-dryer.jpg", true, 1, 9, "Pastikan material kering dan suhu dryer sesuai" },
                    { 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Robot Arm", "/images/checklist/injection/robot-arm.jpg", true, 1, 10, "Cek pergerakan robot arm dan vacuum system" },
                    { 11, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Oil Level", "/images/checklist/injection/oil-level.jpg", true, 1, 11, "Pastikan level oli hydraulic dalam batas normal" },
                    { 12, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cleanliness", "/images/checklist/injection/cleanliness.jpg", true, 1, 12, "Area mesin bersih dari oli, material, dan kotoran" },
                    { 13, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Control Panel", "/images/checklist/injection/control-panel.jpg", true, 1, 13, "Pastikan display dan tombol kontrol berfungsi normal" }
                });

            migrationBuilder.InsertData(
                table: "MachineNumbers",
                columns: new[] { "Id", "CreatedAt", "IsActive", "Location", "MachineId", "Number" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Area A", 1, "1" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Area A", 1, "2" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Area B", 1, "3" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Area C", 2, "1" },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Area C", 2, "2" },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Area D", 3, "1" }
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

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InspectionResults");

            migrationBuilder.DropTable(
                name: "Users");

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
