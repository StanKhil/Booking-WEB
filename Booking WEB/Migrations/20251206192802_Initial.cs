using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Booking_WEB.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RealtyGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealtyGroups", x => x.Id);
                    table.UniqueConstraint("AK_RealtyGroups_Slug", x => x.Slug);
                    table.ForeignKey(
                        name: "FK_RealtyGroups_RealtyGroups_ParentId",
                        column: x => x.ParentId,
                        principalTable: "RealtyGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CanCreate = table.Column<bool>(type: "bit", nullable: false),
                    CanRead = table.Column<bool>(type: "bit", nullable: false),
                    CanUpdate = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardholderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dk = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAccesses_UserRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "UserRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAccesses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Realties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Realties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Realties_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Realties_RealtyGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "RealtyGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "AccRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RealtyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AvgRate = table.Column<float>(type: "real", nullable: false),
                    CountRate = table.Column<int>(type: "int", nullable: false),
                    LastRatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccRates_Realties_RealtyId",
                        column: x => x.RealtyId,
                        principalTable: "Realties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RealtyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserAccessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingItems_Realties_RealtyId",
                        column: x => x.RealtyId,
                        principalTable: "Realties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingItems_UserAccesses_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "UserAccesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RealtyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserAccessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rate = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Realties_RealtyId",
                        column: x => x.RealtyId,
                        principalTable: "Realties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Feedbacks_UserAccesses_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "UserAccesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemImages",
                columns: table => new
                {
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemImages", x => new { x.ItemId, x.ImageUrl });
                    table.ForeignKey(
                        name: "FK_ItemImages_Realties_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Realties",
                        principalColumn: "Id"
                        );
                    table.ForeignKey(
                        name: "FK_ItemImages_RealtyGroups_ItemId",
                        column: x => x.ItemId,
                        principalTable: "RealtyGroups",
                        principalColumn: "Id"
                        );
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"), "Ukraine" },
                    { new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"), "Poland" }
                });

            migrationBuilder.InsertData(
                table: "RealtyGroups",
                columns: new[] { "Id", "DeletedAt", "Description", "ImageUrl", "Name", "ParentId", "Slug" },
                values: new object[,]
                {
                    { new Guid("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"), null, "Villas", "villa.jpg", "Villas", null, "villas" },
                    { new Guid("8806ca58-8daa-4576-92ba-797de42ffaa7"), null, "Apartments", "apartment.jpg", "Apartments", null, "apartments" },
                    { new Guid("97191468-a02f-4a78-927b-9ea660e9ea36"), null, "Houses", "house.jpg", "Houses", null, "houses" },
                    { new Guid("f1ea6b3f-0021-417b-95c8-f6cd333d7207"), null, "Multi-room hotels", "hotel.jpg", "Hotels", null, "hotels" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CanCreate", "CanDelete", "CanRead", "CanUpdate", "Description" },
                values: new object[,]
                {
                    { "Administrator", true, true, true, true, "System administrator" },
                    { "Employee", true, false, true, false, "Company's employee" },
                    { "Moderator", false, true, true, true, "Content editor" },
                    { "SelfRegistered", false, false, false, false, "Self-registered user" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "DeletedAt", "Email", "FirstName", "LastName", "RegisteredAt" },
                values: new object[,]
                {
                    { new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"), new DateTime(1989, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "dnistr@ukr.net", "Дністрянський", "Збоїслав", new DateTime(2024, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("0d156354-89f1-4d58-a735-876b7add59d2"), new DateTime(2005, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "dina@ukr.net", "Гординська", "Діна", new DateTime(2024, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"), new DateTime(1998, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "jakiv@ukr.net", "Палійчук", "Яків", new DateTime(2025, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("a0f7b463-6eef-4a70-8444-789bbea23369"), new DateTime(1999, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "bondarko@ukr.net", "Бондарко", "Юрій", new DateTime(2025, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("a3c55a79-05ea-4053-ad3c-7301f3b7a7e2"), new DateTime(2005, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "romashko@ukr.net", "Ромашко", "Жадан", new DateTime(2024, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"), new DateTime(1999, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "storozh@ukr.net", "Сторож", "Чеслава", new DateTime(2025, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("eadb0b3b-523e-478b-88ee-b6cf57cbc05d"), new DateTime(2001, 12, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "erstenuk@ukr.net", "Ерстенюк", "Вікторія", new DateTime(2025, 1, 21, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "CountryId", "Name" },
                values: new object[,]
                {
                    { new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"), new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"), "Lviv" },
                    { new Guid("59b082e4-19ab-4d7f-a061-4fbc08c59778"), new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"), "Kyiv" },
                    { new Guid("923a6af0-30be-41aa-ae79-fdf41b7bb1b6"), new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"), "Odesa" }
                });

            migrationBuilder.InsertData(
                table: "UserAccesses",
                columns: new[] { "Id", "Dk", "Login", "RoleId", "Salt", "UserId" },
                values: new object[,]
                {
                    { new Guid("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"), "Salt9123", "bondarko", "SelfRegistered", "Salt9", new Guid("a0f7b463-6eef-4a70-8444-789bbea23369") },
                    { new Guid("7a38a3aa-de9f-4519-bb48-eeb86c1efcdf"), "Salt5123", "dina@ukr.net", "Moderator", "Salt5", new Guid("0d156354-89f1-4d58-a735-876b7add59d2") },
                    { new Guid("8806ca58-8daa-4576-92ba-797de42ffaa7"), "Salt7123", "erstenuk", "Employee", "Salt7", new Guid("eadb0b3b-523e-478b-88ee-b6cf57cbc05d") },
                    { new Guid("92cd36b8-ea5a-4cbb-a232-268d942c97fd"), "Salt4123", "dina", "Employee", "Salt4", new Guid("0d156354-89f1-4d58-a735-876b7add59d2") },
                    { new Guid("97191468-a02f-4a78-927b-9ea660e9ea36"), "Salt8123", "erstenuk@ukr.net", "Administrator", "Salt8", new Guid("eadb0b3b-523e-478b-88ee-b6cf57cbc05d") },
                    { new Guid("b31355b7-aa02-4b10-afda-eb9ec8294e78"), "Salt3123", "dnistr", "SelfRegistered", "Salt3", new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434") },
                    { new Guid("e29b6a1a-5bc7-4f42-9fa4-db25de342b42"), "Salt1123", "jakiv", "SelfRegistered", "Salt1", new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d") },
                    { new Guid("f1ea6b3f-0021-417b-95c8-f6cd333d7207"), "Salt6123", "romashko", "SelfRegistered", "Salt6", new Guid("a3c55a79-05ea-4053-ad3c-7301f3b7a7e2") },
                    { new Guid("fb4ad18c-d916-4708-be71-a9bbcf1eb806"), "Salt2123", "storozh", "Employee", "Salt2", new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415") }
                });

            migrationBuilder.InsertData(
                table: "Realties",
                columns: new[] { "Id", "CityId", "DeletedAt", "Description", "GroupId", "Name", "Price", "Slug" },
                values: new object[,]
                {
                    { new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"), new Guid("59b082e4-19ab-4d7f-a061-4fbc08c59778"), null, "Готель \"Лісовий\" - це ідеальне місце для відпочинку на природі.", new Guid("f1ea6b3f-0021-417b-95c8-f6cd333d7207"), "Готель \"Лісовий\"", 250.00m, "hotel-forest" },
                    { new Guid("0d156354-89f1-4d58-a735-876b7add59d2"), new Guid("59b082e4-19ab-4d7f-a061-4fbc08c59778"), null, "Квартира \"Центральна\" - це ідеальне місце для відпочинку в місті.", new Guid("8806ca58-8daa-4576-92ba-797de42ffaa7"), "Квартира \"Центральна\"", 100.00m, "apartment-central" },
                    { new Guid("37dcc68e-b7e7-4b55-b04e-147c1a4126b7"), new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"), null, "Вілла \"Сонячна\" - це ідеальне місце для відпочинку на морі.", new Guid("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"), "Вілла \"Сонячна\"", 500.00m, "villa-sunny" },
                    { new Guid("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"), new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"), null, "Будинок \"Гірський\" - це ідеальне місце для відпочинку в горах.", new Guid("97191468-a02f-4a78-927b-9ea660e9ea36"), "Будинок \"Гірський\"", 400.00m, "house-mountain" },
                    { new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"), new Guid("59b082e4-19ab-4d7f-a061-4fbc08c59778"), null, "Готель \"Сонячний\" - це ідеальне місце для відпочинку на природі.", new Guid("f1ea6b3f-0021-417b-95c8-f6cd333d7207"), "Готель \"Сонячний\"", 150.00m, "hotel-sunny" },
                    { new Guid("a0f7b463-6eef-4a70-8444-789bbea23369"), new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"), null, "Будинок \"Лісовий\" - це ідеальне місце для відпочинку на природі.", new Guid("97191468-a02f-4a78-927b-9ea660e9ea36"), "Будинок \"Лісовий\"", 350.00m, "house-forest" },
                    { new Guid("a3c55a79-05ea-4053-ad3c-7301f3b7a7e2"), new Guid("59b082e4-19ab-4d7f-a061-4fbc08c59778"), null, "Квартира \"Люкс\" - це ідеальне місце для відпочинку, якщо ви не хочете виходити з дому.", new Guid("8806ca58-8daa-4576-92ba-797de42ffaa7"), "Квартира \"Люкс\"", 150.00m, "apartment-luxury" },
                    { new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"), new Guid("59b082e4-19ab-4d7f-a061-4fbc08c59778"), null, "Готель \"Зоряний\" - це ідеальне місце для відпочинку на природі.", new Guid("f1ea6b3f-0021-417b-95c8-f6cd333d7207"), "Готель \"Зоряний\"", 200.00m, "hotel-star" },
                    { new Guid("d5e36e96-0314-4b7e-9cbf-d0fae477ae36"), new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"), null, "Вілла \"Лісова\" - це ідеальне місце для відпочинку на природі.", new Guid("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"), "Вілла \"Лісова\"", 600.00m, "villa-forest" },
                    { new Guid("eadb0b3b-523e-478b-88ee-b6cf57cbc05d"), new Guid("59b082e4-19ab-4d7f-a061-4fbc08c59778"), null, "Будинок \"Садиба\" - це ідеальне місце для відпочинку з друзями.", new Guid("97191468-a02f-4a78-927b-9ea660e9ea36"), "Будинок \"Садиба\"", 300.00m, "house-mansion" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessTokens_Sub",
                table: "AccessTokens",
                column: "Sub");

            migrationBuilder.CreateIndex(
                name: "IX_AccRates_RealtyId",
                table: "AccRates",
                column: "RealtyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_RealtyId",
                table: "BookingItems",
                column: "RealtyId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_UserAccessId",
                table: "BookingItems",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_UserId",
                table: "Cards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_RealtyId",
                table: "Feedbacks",
                column: "RealtyId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_UserAccessId",
                table: "Feedbacks",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_Realties_CityId",
                table: "Realties",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Realties_GroupId",
                table: "Realties",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Realties_Slug",
                table: "Realties",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RealtyGroups_ParentId",
                table: "RealtyGroups",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_Login",
                table: "UserAccesses",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_RoleId",
                table: "UserAccesses",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_UserId",
                table: "UserAccesses",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessTokens");

            migrationBuilder.DropTable(
                name: "AccRates");

            migrationBuilder.DropTable(
                name: "BookingItems");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "ItemImages");

            migrationBuilder.DropTable(
                name: "UserAccesses");

            migrationBuilder.DropTable(
                name: "Realties");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "RealtyGroups");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
