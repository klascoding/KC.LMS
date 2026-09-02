using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KC.LMS.Storage.Migrations
{
    /// <inheritdoc />
    public partial class SeedSystemRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "IsActive", "Name", "TenantId" },
                values: new object[,]
                {
                    { new Guid("6e5a1f5e-0b8a-4f8e-9e2a-000000000001"), "Full access within the tenant.", true, "TenantAdmin", null },
                    { new Guid("6e5a1f5e-0b8a-4f8e-9e2a-000000000002"), "Manages an organization.", true, "OrgManager", null },
                    { new Guid("6e5a1f5e-0b8a-4f8e-9e2a-000000000003"), "Delivers courses.", true, "Instructor", null },
                    { new Guid("6e5a1f5e-0b8a-4f8e-9e2a-000000000004"), "Consumes courses.", true, "Learner", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("6e5a1f5e-0b8a-4f8e-9e2a-000000000001"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("6e5a1f5e-0b8a-4f8e-9e2a-000000000002"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("6e5a1f5e-0b8a-4f8e-9e2a-000000000003"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("6e5a1f5e-0b8a-4f8e-9e2a-000000000004"));
        }
    }
}
