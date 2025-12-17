using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Housing_Architecture.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsVerified", "Name", "Password", "Phone", "RegisteredAt", "Role", "Salt" },
                values: new object[] { 1, "superadmin@housing.uz", true, "SuperAdmin", "mkE1W532sKVzV0GEvOTSrh38SvsiuV+7q0z/izST7N0=", "+998901234567", new DateTime(2025, 12, 16, 7, 17, 3, 642, DateTimeKind.Utc).AddTicks(6917), "Admin", "super-admin-salt-2025" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
