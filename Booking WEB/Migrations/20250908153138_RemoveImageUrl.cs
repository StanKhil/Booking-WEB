using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking_WEB.Migrations
{
    /// <inheritdoc />
    public partial class RemoveImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Realties");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Realties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                column: "ImageUrl",
                value: "hotel_forest.jpg");

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("0d156354-89f1-4d58-a735-876b7add59d2"),
                column: "ImageUrl",
                value: "apartment_central.jpg");

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("37dcc68e-b7e7-4b55-b04e-147c1a4126b7"),
                column: "ImageUrl",
                value: "villa_sunny.jpg");

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"),
                column: "ImageUrl",
                value: "house_mountain.jpg");

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                column: "ImageUrl",
                value: "hotel_sunny.jpg");

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("a0f7b463-6eef-4a70-8444-789bbea23369"),
                column: "ImageUrl",
                value: "house_forest.jpg");

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("a3c55a79-05ea-4053-ad3c-7301f3b7a7e2"),
                column: "ImageUrl",
                value: "apartment_luxury.jpg");

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                column: "ImageUrl",
                value: "hotel_star.jpg");

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("d5e36e96-0314-4b7e-9cbf-d0fae477ae36"),
                column: "ImageUrl",
                value: "villa_forest.jpg");

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("eadb0b3b-523e-478b-88ee-b6cf57cbc05d"),
                column: "ImageUrl",
                value: "house_mansion.jpg");
        }
    }
}
