using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PollingTask.Repository.Migrations
{
    public partial class Seeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1", "bc0f634e-36c1-45dd-9f4e-a2f43a27f261", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2", "5d25d916-bd6c-47bd-b612-54e991f02fa4", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "ec4fc21e-49c2-4391-9c45-d5bbdf9bea69", 0, "9c0efb6a-d9fd-4f4d-8b80-281f11242f6f", "admin@email.com", true, true, null, "ADMIN@EMAIL.COM", "ADMIN@EMAIL.COM", "AQAAAAEAACcQAAAAEDF4Ob9sBQoB4KThEQD1TaVJ0Pqf8Tw4xIAoXi54M87iXW56aS8VRp1xvZLtWsmiPg==", "NULL", false, "d848b95e-3cf3-469e-9f62-4519b618cd30", false, "admin@email.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "ec4fc21e-49c2-4391-9c45-d5bbdf9bea69" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "ec4fc21e-49c2-4391-9c45-d5bbdf9bea69" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ec4fc21e-49c2-4391-9c45-d5bbdf9bea69");
        }
    }
}
