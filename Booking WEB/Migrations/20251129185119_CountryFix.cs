using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Booking_WEB.Migrations
{
    /// <inheritdoc />
    public partial class CountryFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("0d156354-89f1-4d58-a735-876b7add59d2"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d72ad227-ad60-4f30-897f-8a7aaa46e049"));

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("59b082e4-19ab-4d7f-a061-4fbc08c59778"),
                column: "CountryId",
                value: new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"));

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("afd0db5d-9207-42fb-9629-26f5d74ef0b0"),
                column: "CountryId",
                value: new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"));

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("c5efcfde-ee1f-4521-bb8f-f4cdb97c1578"),
                column: "CountryId",
                value: new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("59b082e4-19ab-4d7f-a061-4fbc08c59778"),
                column: "CountryId",
                value: new Guid("d72ad227-ad60-4f30-897f-8a7aaa46e049"));

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("afd0db5d-9207-42fb-9629-26f5d74ef0b0"),
                column: "CountryId",
                value: new Guid("d72ad227-ad60-4f30-897f-8a7aaa46e049"));

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("c5efcfde-ee1f-4521-bb8f-f4cdb97c1578"),
                column: "CountryId",
                value: new Guid("d72ad227-ad60-4f30-897f-8a7aaa46e049"));

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "CountryId", "Name" },
                values: new object[,]
                {
                    { new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"), new Guid("00000000-0000-0000-0000-000000000000"), "Lviv" },
                    { new Guid("0d156354-89f1-4d58-a735-876b7add59d2"), new Guid("00000000-0000-0000-0000-000000000000"), "Krakow" }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("d72ad227-ad60-4f30-897f-8a7aaa46e049"), "Ukraine" });
        }
    }
}
