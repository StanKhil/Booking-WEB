using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking_WEB.Migrations
{
    /// <inheritdoc />
    public partial class CardEdited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CardholderName",
                table: "Cards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardholderName",
                table: "Cards");
        }
    }
}
