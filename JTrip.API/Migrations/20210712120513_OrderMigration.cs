using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JTrip.API.Migrations
{
    public partial class OrderMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "LineItems",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    CreateDateUtc = table.Column<DateTime>(nullable: false),
                    TransactionMetadata = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "028ed6ba-9476-468f-82d3-f696554cb3d9",
                column: "ConcurrencyStamp",
                value: "dfc6c633-d501-4227-a519-6fb06b7e0cff");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "30bedf60-427b-49cd-a521-97a3f0a783a7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fac9b492-82e7-4a98-85a7-c5ec0e616bfb", "AQAAAAEAACcQAAAAEBEX7az1G+18KGuALnHKQLeL30tXAmRUrZz2kPZPnwCRRUPp4OA40LpgJ1FM4Ssnkw==", "efde548c-4622-4cc0-96a7-72809da18a31" });

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_OrderId",
                table: "LineItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LineItems_Orders_OrderId",
                table: "LineItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineItems_Orders_OrderId",
                table: "LineItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_LineItems_OrderId",
                table: "LineItems");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "LineItems");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "028ed6ba-9476-468f-82d3-f696554cb3d9",
                column: "ConcurrencyStamp",
                value: "d778b126-b59e-4bfe-898f-1f85002f81e0");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "30bedf60-427b-49cd-a521-97a3f0a783a7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "91dfe649-4217-4404-ad7c-99a6b7f77017", "AQAAAAEAACcQAAAAEMxJdlGR4DBRLoBULF4o8BOs5TmH05f/1e7/mH2UNkdNpp3/+xluGcJ31QYnOmjwfA==", "52953f95-3f68-45b1-9483-5669fdb80767" });
        }
    }
}
