using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Housing_Architecture.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class PropertyDocument_RejectionReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "PropertyDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegisteredAt",
                value: new DateTime(2026, 1, 16, 13, 27, 30, 865, DateTimeKind.Utc).AddTicks(8762));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "PropertyDocuments");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegisteredAt",
                value: new DateTime(2025, 12, 22, 9, 8, 22, 737, DateTimeKind.Utc).AddTicks(7964));
        }
    }
}
