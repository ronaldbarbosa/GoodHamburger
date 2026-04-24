using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodHamburger.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBevidasCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProductCategories",
                columns: new[] { "Id", "CreatedAt", "IsActive", "Name" },
                values: new object[] { 3, new DateTime(2026, 4, 21, 17, 0, 0, 0, DateTimeKind.Unspecified), true, "Bebidas" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CategoryId",
                value: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CategoryId",
                value: 2);
        }
    }
}
