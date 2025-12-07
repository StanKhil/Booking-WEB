using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking_WEB.Migrations
{
    /// <inheritdoc />
    public partial class BugFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemImages_RealtyGroups_ItemId",
                table: "ItemImages");

            migrationBuilder.AddColumn<Guid>(
                name: "RealtyGroupId",
                table: "ItemImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemImages_RealtyGroupId",
                table: "ItemImages",
                column: "RealtyGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemImages_RealtyGroups_RealtyGroupId",
                table: "ItemImages",
                column: "RealtyGroupId",
                principalTable: "RealtyGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemImages_RealtyGroups_RealtyGroupId",
                table: "ItemImages");

            migrationBuilder.DropIndex(
                name: "IX_ItemImages_RealtyGroupId",
                table: "ItemImages");

            migrationBuilder.DropColumn(
                name: "RealtyGroupId",
                table: "ItemImages");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemImages_RealtyGroups_ItemId",
                table: "ItemImages",
                column: "ItemId",
                principalTable: "RealtyGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
