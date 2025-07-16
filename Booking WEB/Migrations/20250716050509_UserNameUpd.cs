using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking_WEB.Migrations
{
    /// <inheritdoc />
    public partial class UserNameUpd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Дністрянський", "Збоїслав" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0d156354-89f1-4d58-a735-876b7add59d2"),
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Гординська", "Діна" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Палійчук", "Яків" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a0f7b463-6eef-4a70-8444-789bbea23369"),
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Бондарко", "Юрій" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a3c55a79-05ea-4053-ad3c-7301f3b7a7e2"),
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Ромашко", "Жадан" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Сторож", "Чеслава" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("eadb0b3b-523e-478b-88ee-b6cf57cbc05d"),
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Ерстенюк", "Вікторія" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Users",
                newName: "Name");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                column: "Name",
                value: "Дністрянський Збоїслав");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0d156354-89f1-4d58-a735-876b7add59d2"),
                column: "Name",
                value: "Гординська Діна");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                column: "Name",
                value: "Палійчук Яків");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a0f7b463-6eef-4a70-8444-789bbea23369"),
                column: "Name",
                value: "Бондарко Юрій");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a3c55a79-05ea-4053-ad3c-7301f3b7a7e2"),
                column: "Name",
                value: "Ромашко Жадан");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                column: "Name",
                value: "Сторож Чеслава");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("eadb0b3b-523e-478b-88ee-b6cf57cbc05d"),
                column: "Name",
                value: "Ерстенюк Вікторія");
        }
    }
}
