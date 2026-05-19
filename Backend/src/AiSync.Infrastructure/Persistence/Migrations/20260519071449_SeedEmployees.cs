using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AiSync.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedEmployees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "DateOfBirth", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(1990, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Ana García" },
                    { 2, new DateTime(1985, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Carlos López" },
                    { 3, new DateTime(1993, 11, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "María Rodríguez" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "DateOfBirth", "Name" },
                values: new object[] { 4, new DateTime(1988, 1, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jorge Martínez" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "DateOfBirth", "IsActive", "Name" },
                values: new object[,]
                {
                    { 5, new DateTime(1995, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Laura Sánchez" },
                    { 6, new DateTime(1982, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Pedro Hernández" },
                    { 7, new DateTime(1997, 4, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Sofía Torres" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "DateOfBirth", "Name" },
                values: new object[] { 8, new DateTime(1991, 12, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Diego Ramírez" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "DateOfBirth", "IsActive", "Name" },
                values: new object[,]
                {
                    { 9, new DateTime(1994, 8, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Valentina Cruz" },
                    { 10, new DateTime(1987, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Andrés Flores" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
