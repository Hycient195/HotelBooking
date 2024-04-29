using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelBooking.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProjectRestructured : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2bb3bd40-730e-4351-aaa7-43a915b54ba2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fed10253-83d6-42f1-915b-780b9c1bb843");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "340abe80-bed5-4c99-b497-5892f03cc006", null, "User", "USER" },
                    { "71beb3f5-dec1-41fa-937b-fabc3ca65c7b", null, "Administrator", "ADMINISTRATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "340abe80-bed5-4c99-b497-5892f03cc006");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "71beb3f5-dec1-41fa-937b-fabc3ca65c7b");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2bb3bd40-730e-4351-aaa7-43a915b54ba2", null, "User", "USER" },
                    { "fed10253-83d6-42f1-915b-780b9c1bb843", null, "Administrator", "ADMINISTRATOR" }
                });
        }
    }
}
