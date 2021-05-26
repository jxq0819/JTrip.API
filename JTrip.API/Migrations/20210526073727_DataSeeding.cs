using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JTrip.API.Migrations
{
    public partial class DataSeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TouristRoutes",
                columns: new[] { "Id", "CreateTime", "DepartureTime", "Description", "DiscountPercent", "Features", "Fees", "Notes", "OriginalPrice", "Title", "UpdateTime" },
                values: new object[] { new Guid("b908ddb3-78ee-46a7-bb49-cc8c67aac28a"), new DateTime(2021, 5, 26, 7, 37, 27, 306, DateTimeKind.Utc).AddTicks(335), null, "TestDescription", null, null, null, null, 0m, "TestTitle", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TouristRoutes",
                keyColumn: "Id",
                keyValue: new Guid("b908ddb3-78ee-46a7-bb49-cc8c67aac28a"));
        }
    }
}
