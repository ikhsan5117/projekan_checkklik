using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRVI.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataPlants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users_BTR",
                columns: new[] { "Id", "CreatedAt", "Department", "Email", "FullName", "IsActive", "LastLogin", "Password", "Role", "Username" },
                values: new object[] { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "IT", "admin.btr@amrvi.com", "Admin BTR", true, null, "admin123", "Admin", "admin" });

            migrationBuilder.InsertData(
                table: "Users_HOSE",
                columns: new[] { "Id", "CreatedAt", "Department", "Email", "FullName", "IsActive", "LastLogin", "Password", "Role", "Username" },
                values: new object[] { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "IT", "admin.hose@amrvi.com", "Admin HOSE", true, null, "admin123", "Admin", "admin" });

            migrationBuilder.InsertData(
                table: "Users_MIXING",
                columns: new[] { "Id", "CreatedAt", "Department", "Email", "FullName", "IsActive", "LastLogin", "Password", "Role", "Username" },
                values: new object[] { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "IT", "admin.mixing@amrvi.com", "Admin MIXING", true, null, "admin123", "Admin", "admin" });

            migrationBuilder.InsertData(
                table: "Users_MOLDED",
                columns: new[] { "Id", "CreatedAt", "Department", "Email", "FullName", "IsActive", "LastLogin", "Password", "Role", "Username" },
                values: new object[] { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "IT", "admin.molded@amrvi.com", "Admin MOLDED", true, null, "admin123", "Admin", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users_BTR",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users_HOSE",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users_MIXING",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users_MOLDED",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
