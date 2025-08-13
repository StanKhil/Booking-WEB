using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking_WEB.Migrations
{
    /// <inheritdoc />
    public partial class AccessToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessTokens",
                columns: table => new
                {
                    Jti = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Sub = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Iat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nbf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aud = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Iss = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessTokens", x => x.Jti);
                    table.ForeignKey(
                        name: "FK_AccessTokens_UserAccesses_Sub",
                        column: x => x.Sub,
                        principalTable: "UserAccesses",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: "Administrator",
                column: "Description",
                value: "System administrator");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: "Employee",
                column: "Description",
                value: "Company's employee");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: "Moderator",
                column: "Description",
                value: "Content editor");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: "SelfRegistered",
                column: "Description",
                value: "Self-registered user");

            migrationBuilder.CreateIndex(
                name: "IX_AccessTokens_Sub",
                table: "AccessTokens",
                column: "Sub");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessTokens");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: "Administrator",
                column: "Description",
                value: "Системний адміністратор");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: "Employee",
                column: "Description",
                value: "Співробітник компанії");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: "Moderator",
                column: "Description",
                value: "Редактор контенту");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: "SelfRegistered",
                column: "Description",
                value: "Самостійно зареєстрований користувач");
        }
    }
}
