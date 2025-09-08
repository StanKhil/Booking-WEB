using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Booking_WEB.Migrations
{
    /// <inheritdoc />
    public partial class CityCountryEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("d72ad227-ad60-4f30-897f-8a7aaa46e049"), "Ukraine" });

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                column: "CityId",
                value: new Guid("afd0db5d-9207-42fb-9629-26f5d74ef0b0"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("0d156354-89f1-4d58-a735-876b7add59d2"),
                column: "CityId",
                value: new Guid("afd0db5d-9207-42fb-9629-26f5d74ef0b0"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("37dcc68e-b7e7-4b55-b04e-147c1a4126b7"),
                column: "CityId",
                value: new Guid("c5efcfde-ee1f-4521-bb8f-f4cdb97c1578"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"),
                column: "CityId",
                value: new Guid("c5efcfde-ee1f-4521-bb8f-f4cdb97c1578"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                column: "CityId",
                value: new Guid("59b082e4-19ab-4d7f-a061-4fbc08c59778"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("a0f7b463-6eef-4a70-8444-789bbea23369"),
                column: "CityId",
                value: new Guid("c5efcfde-ee1f-4521-bb8f-f4cdb97c1578"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("a3c55a79-05ea-4053-ad3c-7301f3b7a7e2"),
                column: "CityId",
                value: new Guid("c5efcfde-ee1f-4521-bb8f-f4cdb97c1578"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                column: "CityId",
                value: new Guid("59b082e4-19ab-4d7f-a061-4fbc08c59778"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("d5e36e96-0314-4b7e-9cbf-d0fae477ae36"),
                column: "CityId",
                value: new Guid("c5efcfde-ee1f-4521-bb8f-f4cdb97c1578"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("eadb0b3b-523e-478b-88ee-b6cf57cbc05d"),
                column: "CityId",
                value: new Guid("c5efcfde-ee1f-4521-bb8f-f4cdb97c1578"));

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "CountryId", "Name" },
                values: new object[,]
                {
                    { new Guid("59b082e4-19ab-4d7f-a061-4fbc08c59778"), new Guid("d72ad227-ad60-4f30-897f-8a7aaa46e049"), "Kyiv" },
                    { new Guid("afd0db5d-9207-42fb-9629-26f5d74ef0b0"), new Guid("d72ad227-ad60-4f30-897f-8a7aaa46e049"), "Lviv" },
                    { new Guid("c5efcfde-ee1f-4521-bb8f-f4cdb97c1578"), new Guid("d72ad227-ad60-4f30-897f-8a7aaa46e049"), "Odesa" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("59b082e4-19ab-4d7f-a061-4fbc08c59778"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("afd0db5d-9207-42fb-9629-26f5d74ef0b0"));

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("c5efcfde-ee1f-4521-bb8f-f4cdb97c1578"));

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: new Guid("d72ad227-ad60-4f30-897f-8a7aaa46e049"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                column: "CityId",
                value: new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("0d156354-89f1-4d58-a735-876b7add59d2"),
                column: "CityId",
                value: new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("37dcc68e-b7e7-4b55-b04e-147c1a4126b7"),
                column: "CityId",
                value: new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"),
                column: "CityId",
                value: new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                column: "CityId",
                value: new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("a0f7b463-6eef-4a70-8444-789bbea23369"),
                column: "CityId",
                value: new Guid("0d156354-89f1-4d58-a735-876b7add59d2"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("a3c55a79-05ea-4053-ad3c-7301f3b7a7e2"),
                column: "CityId",
                value: new Guid("0d156354-89f1-4d58-a735-876b7add59d2"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                column: "CityId",
                value: new Guid("0d156354-89f1-4d58-a735-876b7add59d2"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("d5e36e96-0314-4b7e-9cbf-d0fae477ae36"),
                column: "CityId",
                value: new Guid("0d156354-89f1-4d58-a735-876b7add59d2"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("eadb0b3b-523e-478b-88ee-b6cf57cbc05d"),
                column: "CityId",
                value: new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"));
        }
    }
}
