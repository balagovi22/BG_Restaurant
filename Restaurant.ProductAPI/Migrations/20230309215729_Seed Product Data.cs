using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.ProductAPI.Migrations
{
    public partial class SeedProductData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "Id", "CategoryName", "Description", "ImageUrl", "Name", "Price" },
                values: new object[] { 1, "Appetizer", "An Indian Appetizer", "https://bgmicrostorage.blob.core.windows.net/images/11[1].jpg", "Samosa", 5.9900000000000002 });

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "Id", "CategoryName", "Description", "ImageUrl", "Name", "Price" },
                values: new object[] { 2, null, "An Indian Appetizer", "https://bgmicrostorage.blob.core.windows.net/images/12[1].jpg", "Gobi Manchurian", 12.99 });

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "Id", "CategoryName", "Description", "ImageUrl", "Name", "Price" },
                values: new object[] { 3, "Appetizer", "An Indian Appetizer made with delicious spices", "", "Pani Pori", 7.9900000000000002 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
