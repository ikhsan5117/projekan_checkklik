using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AMRVI.Migrations
{
    /// <inheritdoc />
    public partial class InitialMultiPlantSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Machines_BTR",
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
                    table.PrimaryKey("PK_Machines_BTR", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Machines_HOSE",
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
                    table.PrimaryKey("PK_Machines_HOSE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Machines_MIXING",
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
                    table.PrimaryKey("PK_Machines_MIXING", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Machines_MOLDED",
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
                    table.PrimaryKey("PK_Machines_MOLDED", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Machines_RVI",
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
                    table.PrimaryKey("PK_Machines_RVI", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users_BTR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users_BTR", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users_HOSE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users_HOSE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users_MIXING",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users_MIXING", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users_MOLDED",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users_MOLDED", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users_RVI",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users_RVI", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItems_BTR",
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
                    table.PrimaryKey("PK_ChecklistItems_BTR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistItems_BTR_Machines_BTR_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines_BTR",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MachineNumbers_BTR",
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
                    table.PrimaryKey("PK_MachineNumbers_BTR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MachineNumbers_BTR_Machines_BTR_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines_BTR",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItems_HOSE",
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
                    table.PrimaryKey("PK_ChecklistItems_HOSE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistItems_HOSE_Machines_HOSE_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines_HOSE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MachineNumbers_HOSE",
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
                    table.PrimaryKey("PK_MachineNumbers_HOSE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MachineNumbers_HOSE_Machines_HOSE_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines_HOSE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItems_MIXING",
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
                    table.PrimaryKey("PK_ChecklistItems_MIXING", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistItems_MIXING_Machines_MIXING_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines_MIXING",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MachineNumbers_MIXING",
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
                    table.PrimaryKey("PK_MachineNumbers_MIXING", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MachineNumbers_MIXING_Machines_MIXING_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines_MIXING",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItems_MOLDED",
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
                    table.PrimaryKey("PK_ChecklistItems_MOLDED", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistItems_MOLDED_Machines_MOLDED_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines_MOLDED",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MachineNumbers_MOLDED",
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
                    table.PrimaryKey("PK_MachineNumbers_MOLDED", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MachineNumbers_MOLDED_Machines_MOLDED_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines_MOLDED",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItems_RVI",
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
                    table.PrimaryKey("PK_ChecklistItems_RVI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistItems_RVI_Machines_RVI_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines_RVI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MachineNumbers_RVI",
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
                    table.PrimaryKey("PK_MachineNumbers_RVI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MachineNumbers_RVI_Machines_RVI_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines_RVI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionSessions_BTR",
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
                    table.PrimaryKey("PK_InspectionSessions_BTR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionSessions_BTR_MachineNumbers_BTR_MachineNumberId",
                        column: x => x.MachineNumberId,
                        principalTable: "MachineNumbers_BTR",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionSessions_HOSE",
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
                    table.PrimaryKey("PK_InspectionSessions_HOSE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionSessions_HOSE_MachineNumbers_HOSE_MachineNumberId",
                        column: x => x.MachineNumberId,
                        principalTable: "MachineNumbers_HOSE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionSessions_MIXING",
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
                    table.PrimaryKey("PK_InspectionSessions_MIXING", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionSessions_MIXING_MachineNumbers_MIXING_MachineNumberId",
                        column: x => x.MachineNumberId,
                        principalTable: "MachineNumbers_MIXING",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionSessions_MOLDED",
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
                    table.PrimaryKey("PK_InspectionSessions_MOLDED", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionSessions_MOLDED_MachineNumbers_MOLDED_MachineNumberId",
                        column: x => x.MachineNumberId,
                        principalTable: "MachineNumbers_MOLDED",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionSessions_RVI",
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
                    table.PrimaryKey("PK_InspectionSessions_RVI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionSessions_RVI_MachineNumbers_RVI_MachineNumberId",
                        column: x => x.MachineNumberId,
                        principalTable: "MachineNumbers_RVI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspectionResults_BTR",
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
                    table.PrimaryKey("PK_InspectionResults_BTR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionResults_BTR_ChecklistItems_BTR_ChecklistItemId",
                        column: x => x.ChecklistItemId,
                        principalTable: "ChecklistItems_BTR",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectionResults_BTR_InspectionSessions_BTR_InspectionSessionId",
                        column: x => x.InspectionSessionId,
                        principalTable: "InspectionSessions_BTR",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InspectionResults_HOSE",
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
                    table.PrimaryKey("PK_InspectionResults_HOSE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionResults_HOSE_ChecklistItems_HOSE_ChecklistItemId",
                        column: x => x.ChecklistItemId,
                        principalTable: "ChecklistItems_HOSE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectionResults_HOSE_InspectionSessions_HOSE_InspectionSessionId",
                        column: x => x.InspectionSessionId,
                        principalTable: "InspectionSessions_HOSE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InspectionResults_MIXING",
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
                    table.PrimaryKey("PK_InspectionResults_MIXING", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionResults_MIXING_ChecklistItems_MIXING_ChecklistItemId",
                        column: x => x.ChecklistItemId,
                        principalTable: "ChecklistItems_MIXING",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectionResults_MIXING_InspectionSessions_MIXING_InspectionSessionId",
                        column: x => x.InspectionSessionId,
                        principalTable: "InspectionSessions_MIXING",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InspectionResults_MOLDED",
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
                    table.PrimaryKey("PK_InspectionResults_MOLDED", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionResults_MOLDED_ChecklistItems_MOLDED_ChecklistItemId",
                        column: x => x.ChecklistItemId,
                        principalTable: "ChecklistItems_MOLDED",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectionResults_MOLDED_InspectionSessions_MOLDED_InspectionSessionId",
                        column: x => x.InspectionSessionId,
                        principalTable: "InspectionSessions_MOLDED",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InspectionResults_RVI",
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
                    table.PrimaryKey("PK_InspectionResults_RVI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionResults_RVI_ChecklistItems_RVI_ChecklistItemId",
                        column: x => x.ChecklistItemId,
                        principalTable: "ChecklistItems_RVI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InspectionResults_RVI_InspectionSessions_RVI_InspectionSessionId",
                        column: x => x.InspectionSessionId,
                        principalTable: "InspectionSessions_RVI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Machines_RVI",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Injection Molding Machine", true, "MESIN INJECTION" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Press Machine", true, "MESIN PRESS" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CNC Machine", true, "MESIN CNC" }
                });

            migrationBuilder.InsertData(
                table: "Users_RVI",
                columns: new[] { "Id", "CreatedAt", "Department", "Email", "FullName", "IsActive", "LastLogin", "Password", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "IT", "admin@amrvi.com", "Administrator", true, null, "admin123", "Admin", "admin" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Production", "supervisor@amrvi.com", "Supervisor Production", true, null, "super123", "Supervisor", "supervisor" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Production", "operator1@amrvi.com", "Operator Mesin 1", true, null, "user123", "User", "operator1" }
                });

            migrationBuilder.InsertData(
                table: "ChecklistItems_RVI",
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
                table: "MachineNumbers_RVI",
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
                name: "IX_ChecklistItems_BTR_MachineId",
                table: "ChecklistItems_BTR",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItems_HOSE_MachineId",
                table: "ChecklistItems_HOSE",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItems_MIXING_MachineId",
                table: "ChecklistItems_MIXING",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItems_MOLDED_MachineId",
                table: "ChecklistItems_MOLDED",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItems_RVI_MachineId",
                table: "ChecklistItems_RVI",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionResults_BTR_ChecklistItemId",
                table: "InspectionResults_BTR",
                column: "ChecklistItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionResults_BTR_InspectionSessionId",
                table: "InspectionResults_BTR",
                column: "InspectionSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionResults_HOSE_ChecklistItemId",
                table: "InspectionResults_HOSE",
                column: "ChecklistItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionResults_HOSE_InspectionSessionId",
                table: "InspectionResults_HOSE",
                column: "InspectionSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionResults_MIXING_ChecklistItemId",
                table: "InspectionResults_MIXING",
                column: "ChecklistItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionResults_MIXING_InspectionSessionId",
                table: "InspectionResults_MIXING",
                column: "InspectionSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionResults_MOLDED_ChecklistItemId",
                table: "InspectionResults_MOLDED",
                column: "ChecklistItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionResults_MOLDED_InspectionSessionId",
                table: "InspectionResults_MOLDED",
                column: "InspectionSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionResults_RVI_ChecklistItemId",
                table: "InspectionResults_RVI",
                column: "ChecklistItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionResults_RVI_InspectionSessionId",
                table: "InspectionResults_RVI",
                column: "InspectionSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionSessions_BTR_MachineNumberId",
                table: "InspectionSessions_BTR",
                column: "MachineNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionSessions_HOSE_MachineNumberId",
                table: "InspectionSessions_HOSE",
                column: "MachineNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionSessions_MIXING_MachineNumberId",
                table: "InspectionSessions_MIXING",
                column: "MachineNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionSessions_MOLDED_MachineNumberId",
                table: "InspectionSessions_MOLDED",
                column: "MachineNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionSessions_RVI_MachineNumberId",
                table: "InspectionSessions_RVI",
                column: "MachineNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_MachineNumbers_BTR_MachineId",
                table: "MachineNumbers_BTR",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_MachineNumbers_HOSE_MachineId",
                table: "MachineNumbers_HOSE",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_MachineNumbers_MIXING_MachineId",
                table: "MachineNumbers_MIXING",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_MachineNumbers_MOLDED_MachineId",
                table: "MachineNumbers_MOLDED",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_MachineNumbers_RVI_MachineId",
                table: "MachineNumbers_RVI",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Machines_RVI_Name",
                table: "Machines_RVI",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BTR_Username",
                table: "Users_BTR",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_HOSE_Username",
                table: "Users_HOSE",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_MIXING_Username",
                table: "Users_MIXING",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_MOLDED_Username",
                table: "Users_MOLDED",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RVI_Email",
                table: "Users_RVI",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RVI_Username",
                table: "Users_RVI",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InspectionResults_BTR");

            migrationBuilder.DropTable(
                name: "InspectionResults_HOSE");

            migrationBuilder.DropTable(
                name: "InspectionResults_MIXING");

            migrationBuilder.DropTable(
                name: "InspectionResults_MOLDED");

            migrationBuilder.DropTable(
                name: "InspectionResults_RVI");

            migrationBuilder.DropTable(
                name: "Users_BTR");

            migrationBuilder.DropTable(
                name: "Users_HOSE");

            migrationBuilder.DropTable(
                name: "Users_MIXING");

            migrationBuilder.DropTable(
                name: "Users_MOLDED");

            migrationBuilder.DropTable(
                name: "Users_RVI");

            migrationBuilder.DropTable(
                name: "ChecklistItems_BTR");

            migrationBuilder.DropTable(
                name: "InspectionSessions_BTR");

            migrationBuilder.DropTable(
                name: "ChecklistItems_HOSE");

            migrationBuilder.DropTable(
                name: "InspectionSessions_HOSE");

            migrationBuilder.DropTable(
                name: "ChecklistItems_MIXING");

            migrationBuilder.DropTable(
                name: "InspectionSessions_MIXING");

            migrationBuilder.DropTable(
                name: "ChecklistItems_MOLDED");

            migrationBuilder.DropTable(
                name: "InspectionSessions_MOLDED");

            migrationBuilder.DropTable(
                name: "ChecklistItems_RVI");

            migrationBuilder.DropTable(
                name: "InspectionSessions_RVI");

            migrationBuilder.DropTable(
                name: "MachineNumbers_BTR");

            migrationBuilder.DropTable(
                name: "MachineNumbers_HOSE");

            migrationBuilder.DropTable(
                name: "MachineNumbers_MIXING");

            migrationBuilder.DropTable(
                name: "MachineNumbers_MOLDED");

            migrationBuilder.DropTable(
                name: "MachineNumbers_RVI");

            migrationBuilder.DropTable(
                name: "Machines_BTR");

            migrationBuilder.DropTable(
                name: "Machines_HOSE");

            migrationBuilder.DropTable(
                name: "Machines_MIXING");

            migrationBuilder.DropTable(
                name: "Machines_MOLDED");

            migrationBuilder.DropTable(
                name: "Machines_RVI");
        }
    }
}
