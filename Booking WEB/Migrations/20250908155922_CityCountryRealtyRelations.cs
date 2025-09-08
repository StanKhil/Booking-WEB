using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking_WEB.Migrations
{
    /// <inheritdoc />
    public partial class CityCountryRealtyRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Realties_Countries_CountryId",
                table: "Realties");

            migrationBuilder.AlterColumn<Guid>(
                name: "CountryId",
                table: "Realties",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "CountryId",
                table: "Cities",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                column: "CountryId",
                value: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: new Guid("0d156354-89f1-4d58-a735-876b7add59d2"),
                column: "CountryId",
                value: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                column: "CountryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("0d156354-89f1-4d58-a735-876b7add59d2"),
                column: "CountryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("37dcc68e-b7e7-4b55-b04e-147c1a4126b7"),
                column: "CountryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"),
                column: "CountryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                column: "CountryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("a0f7b463-6eef-4a70-8444-789bbea23369"),
                column: "CountryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("a3c55a79-05ea-4053-ad3c-7301f3b7a7e2"),
                column: "CountryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                column: "CountryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("d5e36e96-0314-4b7e-9cbf-d0fae477ae36"),
                column: "CountryId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("eadb0b3b-523e-478b-88ee-b6cf57cbc05d"),
                column: "CountryId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Countries_CountryId",
                table: "Cities",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Realties_Countries_CountryId",
                table: "Realties",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Countries_CountryId",
                table: "Cities");

            migrationBuilder.DropForeignKey(
                name: "FK_Realties_Countries_CountryId",
                table: "Realties");

            migrationBuilder.DropIndex(
                name: "IX_Cities_CountryId",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Cities");

            migrationBuilder.AlterColumn<Guid>(
                name: "CountryId",
                table: "Realties",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                column: "CountryId",
                value: new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("0d156354-89f1-4d58-a735-876b7add59d2"),
                column: "CountryId",
                value: new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("37dcc68e-b7e7-4b55-b04e-147c1a4126b7"),
                column: "CountryId",
                value: new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"),
                column: "CountryId",
                value: new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                column: "CountryId",
                value: new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("a0f7b463-6eef-4a70-8444-789bbea23369"),
                column: "CountryId",
                value: new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("a3c55a79-05ea-4053-ad3c-7301f3b7a7e2"),
                column: "CountryId",
                value: new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                column: "CountryId",
                value: new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("d5e36e96-0314-4b7e-9cbf-d0fae477ae36"),
                column: "CountryId",
                value: new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"));

            migrationBuilder.UpdateData(
                table: "Realties",
                keyColumn: "Id",
                keyValue: new Guid("eadb0b3b-523e-478b-88ee-b6cf57cbc05d"),
                column: "CountryId",
                value: new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"));

            migrationBuilder.AddForeignKey(
                name: "FK_Realties_Countries_CountryId",
                table: "Realties",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
