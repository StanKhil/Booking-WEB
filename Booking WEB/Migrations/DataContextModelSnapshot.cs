﻿// <auto-generated />
using System;
using Booking_WEB.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Booking_WEB.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Booking_WEB.Data.Entities.AccRates", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<float>("AvgRate")
                        .HasColumnType("real");

                    b.Property<int>("CountRate")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastRatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("RealtyId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RealtyId")
                        .IsUnique();

                    b.ToTable("AccRates");
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.BookingItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("RealtyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserAccessId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RealtyId");

                    b.HasIndex("UserAccessId");

                    b.ToTable("BookingItems");
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.City", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Cities");

                    b.HasData(
                        new
                        {
                            Id = new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                            Name = "Lviv"
                        },
                        new
                        {
                            Id = new Guid("0d156354-89f1-4d58-a735-876b7add59d2"),
                            Name = "Krakow"
                        });
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.Country", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Countries");

                    b.HasData(
                        new
                        {
                            Id = new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                            Name = "Ukraine"
                        },
                        new
                        {
                            Id = new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                            Name = "Poland"
                        });
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.Feedback", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Rate")
                        .HasColumnType("int");

                    b.Property<Guid>("RealtyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserAccessId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RealtyId");

                    b.HasIndex("UserAccessId");

                    b.ToTable("Feedbacks");
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.ItemImage", b =>
                {
                    b.Property<Guid>("ItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("Order")
                        .HasColumnType("int");

                    b.HasKey("ItemId", "ImageUrl");

                    b.ToTable("ItemImages");
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.Realty", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CountryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(12,2)");

                    b.Property<string>("Slug")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.HasIndex("CountryId");

                    b.HasIndex("GroupId");

                    b.HasIndex("Slug")
                        .IsUnique()
                        .HasFilter("[Slug] IS NOT NULL");

                    b.ToTable("Realties");

                    b.HasData(
                        new
                        {
                            Id = new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                            CityId = new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                            CountryId = new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                            Description = "Готель \"Сонячний\" - це ідеальне місце для відпочинку на природі.",
                            GroupId = new Guid("f1ea6b3f-0021-417b-95c8-f6cd333d7207"),
                            ImageUrl = "hotel_sunny.jpg",
                            Name = "Готель \"Сонячний\"",
                            Price = 150.00m,
                            Slug = "hotel-sunny"
                        },
                        new
                        {
                            Id = new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                            CityId = new Guid("0d156354-89f1-4d58-a735-876b7add59d2"),
                            CountryId = new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                            Description = "Готель \"Зоряний\" - це ідеальне місце для відпочинку на природі.",
                            GroupId = new Guid("f1ea6b3f-0021-417b-95c8-f6cd333d7207"),
                            ImageUrl = "hotel_star.jpg",
                            Name = "Готель \"Зоряний\"",
                            Price = 200.00m,
                            Slug = "hotel-star"
                        },
                        new
                        {
                            Id = new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                            CityId = new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                            CountryId = new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                            Description = "Готель \"Лісовий\" - це ідеальне місце для відпочинку на природі.",
                            GroupId = new Guid("f1ea6b3f-0021-417b-95c8-f6cd333d7207"),
                            ImageUrl = "hotel_forest.jpg",
                            Name = "Готель \"Лісовий\"",
                            Price = 250.00m,
                            Slug = "hotel-forest"
                        },
                        new
                        {
                            Id = new Guid("0d156354-89f1-4d58-a735-876b7add59d2"),
                            CityId = new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                            CountryId = new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                            Description = "Квартира \"Центральна\" - це ідеальне місце для відпочинку в місті.",
                            GroupId = new Guid("8806ca58-8daa-4576-92ba-797de42ffaa7"),
                            ImageUrl = "apartment_central.jpg",
                            Name = "Квартира \"Центральна\"",
                            Price = 100.00m,
                            Slug = "apartment-central"
                        },
                        new
                        {
                            Id = new Guid("a3c55a79-05ea-4053-ad3c-7301f3b7a7e2"),
                            CityId = new Guid("0d156354-89f1-4d58-a735-876b7add59d2"),
                            CountryId = new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                            Description = "Квартира \"Люкс\" - це ідеальне місце для відпочинку, якщо ви не хочете виходити з дому.",
                            GroupId = new Guid("8806ca58-8daa-4576-92ba-797de42ffaa7"),
                            ImageUrl = "apartment_luxury.jpg",
                            Name = "Квартира \"Люкс\"",
                            Price = 150.00m,
                            Slug = "apartment-luxury"
                        },
                        new
                        {
                            Id = new Guid("eadb0b3b-523e-478b-88ee-b6cf57cbc05d"),
                            CityId = new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                            CountryId = new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                            Description = "Будинок \"Садиба\" - це ідеальне місце для відпочинку з друзями.",
                            GroupId = new Guid("97191468-a02f-4a78-927b-9ea660e9ea36"),
                            ImageUrl = "house_mansion.jpg",
                            Name = "Будинок \"Садиба\"",
                            Price = 300.00m,
                            Slug = "house-mansion"
                        },
                        new
                        {
                            Id = new Guid("a0f7b463-6eef-4a70-8444-789bbea23369"),
                            CityId = new Guid("0d156354-89f1-4d58-a735-876b7add59d2"),
                            CountryId = new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                            Description = "Будинок \"Лісовий\" - це ідеальне місце для відпочинку на природі.",
                            GroupId = new Guid("97191468-a02f-4a78-927b-9ea660e9ea36"),
                            ImageUrl = "house_forest.jpg",
                            Name = "Будинок \"Лісовий\"",
                            Price = 350.00m,
                            Slug = "house-forest"
                        },
                        new
                        {
                            Id = new Guid("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"),
                            CityId = new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                            CountryId = new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                            Description = "Будинок \"Гірський\" - це ідеальне місце для відпочинку в горах.",
                            GroupId = new Guid("97191468-a02f-4a78-927b-9ea660e9ea36"),
                            ImageUrl = "house_mountain.jpg",
                            Name = "Будинок \"Гірський\"",
                            Price = 400.00m,
                            Slug = "house-mountain"
                        },
                        new
                        {
                            Id = new Guid("37dcc68e-b7e7-4b55-b04e-147c1a4126b7"),
                            CityId = new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                            CountryId = new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                            Description = "Вілла \"Сонячна\" - це ідеальне місце для відпочинку на морі.",
                            GroupId = new Guid("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"),
                            ImageUrl = "villa_sunny.jpg",
                            Name = "Вілла \"Сонячна\"",
                            Price = 500.00m,
                            Slug = "villa-sunny"
                        },
                        new
                        {
                            Id = new Guid("d5e36e96-0314-4b7e-9cbf-d0fae477ae36"),
                            CityId = new Guid("0d156354-89f1-4d58-a735-876b7add59d2"),
                            CountryId = new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                            Description = "Вілла \"Лісова\" - це ідеальне місце для відпочинку на природі.",
                            GroupId = new Guid("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"),
                            ImageUrl = "villa_forest.jpg",
                            Name = "Вілла \"Лісова\"",
                            Price = 600.00m,
                            Slug = "villa-forest"
                        });
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.RealtyGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasAlternateKey("Slug");

                    b.HasIndex("ParentId");

                    b.ToTable("RealtyGroups");

                    b.HasData(
                        new
                        {
                            Id = new Guid("f1ea6b3f-0021-417b-95c8-f6cd333d7207"),
                            Description = "Multi-room hotels",
                            ImageUrl = "hotel.jpg",
                            Name = "Hotels",
                            Slug = "hotels"
                        },
                        new
                        {
                            Id = new Guid("8806ca58-8daa-4576-92ba-797de42ffaa7"),
                            Description = "Apartments",
                            ImageUrl = "apartment.jpg",
                            Name = "Apartments",
                            Slug = "apartments"
                        },
                        new
                        {
                            Id = new Guid("97191468-a02f-4a78-927b-9ea660e9ea36"),
                            Description = "Houses",
                            ImageUrl = "house.jpg",
                            Name = "Houses",
                            Slug = "houses"
                        },
                        new
                        {
                            Id = new Guid("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"),
                            Description = "Villas",
                            ImageUrl = "villa.jpg",
                            Name = "Villas",
                            Slug = "villas"
                        });
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.UserAccess", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Dk")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserAccesses");

                    b.HasData(
                        new
                        {
                            Id = new Guid("e29b6a1a-5bc7-4f42-9fa4-db25de342b42"),
                            Dk = "Salt1123",
                            Login = "jakiv",
                            RoleId = "SelfRegistered",
                            Salt = "Salt1",
                            UserId = new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d")
                        },
                        new
                        {
                            Id = new Guid("fb4ad18c-d916-4708-be71-a9bbcf1eb806"),
                            Dk = "Salt2123",
                            Login = "storozh",
                            RoleId = "Employee",
                            Salt = "Salt2",
                            UserId = new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415")
                        },
                        new
                        {
                            Id = new Guid("b31355b7-aa02-4b10-afda-eb9ec8294e78"),
                            Dk = "Salt3123",
                            Login = "dnistr",
                            RoleId = "SelfRegistered",
                            Salt = "Salt3",
                            UserId = new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434")
                        },
                        new
                        {
                            Id = new Guid("92cd36b8-ea5a-4cbb-a232-268d942c97fd"),
                            Dk = "Salt4123",
                            Login = "dina",
                            RoleId = "Employee",
                            Salt = "Salt4",
                            UserId = new Guid("0d156354-89f1-4d58-a735-876b7add59d2")
                        },
                        new
                        {
                            Id = new Guid("7a38a3aa-de9f-4519-bb48-eeb86c1efcdf"),
                            Dk = "Salt5123",
                            Login = "dina@ukr.net",
                            RoleId = "Moderator",
                            Salt = "Salt5",
                            UserId = new Guid("0d156354-89f1-4d58-a735-876b7add59d2")
                        },
                        new
                        {
                            Id = new Guid("f1ea6b3f-0021-417b-95c8-f6cd333d7207"),
                            Dk = "Salt6123",
                            Login = "romashko",
                            RoleId = "SelfRegistered",
                            Salt = "Salt6",
                            UserId = new Guid("a3c55a79-05ea-4053-ad3c-7301f3b7a7e2")
                        },
                        new
                        {
                            Id = new Guid("8806ca58-8daa-4576-92ba-797de42ffaa7"),
                            Dk = "Salt7123",
                            Login = "erstenuk",
                            RoleId = "Employee",
                            Salt = "Salt7",
                            UserId = new Guid("eadb0b3b-523e-478b-88ee-b6cf57cbc05d")
                        },
                        new
                        {
                            Id = new Guid("97191468-a02f-4a78-927b-9ea660e9ea36"),
                            Dk = "Salt8123",
                            Login = "erstenuk@ukr.net",
                            RoleId = "Administrator",
                            Salt = "Salt8",
                            UserId = new Guid("eadb0b3b-523e-478b-88ee-b6cf57cbc05d")
                        },
                        new
                        {
                            Id = new Guid("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"),
                            Dk = "Salt9123",
                            Login = "bondarko",
                            RoleId = "SelfRegistered",
                            Salt = "Salt9",
                            UserId = new Guid("a0f7b463-6eef-4a70-8444-789bbea23369")
                        });
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.UserData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RegisteredAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = new Guid("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                            BirthDate = new DateTime(1998, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "jakiv@ukr.net",
                            FirstName = "Палійчук",
                            LastName = "Яків",
                            RegisteredAt = new DateTime(2025, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            Id = new Guid("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                            BirthDate = new DateTime(1999, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "storozh@ukr.net",
                            FirstName = "Сторож",
                            LastName = "Чеслава",
                            RegisteredAt = new DateTime(2025, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            Id = new Guid("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                            BirthDate = new DateTime(1989, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "dnistr@ukr.net",
                            FirstName = "Дністрянський",
                            LastName = "Збоїслав",
                            RegisteredAt = new DateTime(2024, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            Id = new Guid("0d156354-89f1-4d58-a735-876b7add59d2"),
                            BirthDate = new DateTime(2005, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "dina@ukr.net",
                            FirstName = "Гординська",
                            LastName = "Діна",
                            RegisteredAt = new DateTime(2024, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            Id = new Guid("a3c55a79-05ea-4053-ad3c-7301f3b7a7e2"),
                            BirthDate = new DateTime(2005, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "romashko@ukr.net",
                            FirstName = "Ромашко",
                            LastName = "Жадан",
                            RegisteredAt = new DateTime(2024, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            Id = new Guid("eadb0b3b-523e-478b-88ee-b6cf57cbc05d"),
                            BirthDate = new DateTime(2001, 12, 21, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "erstenuk@ukr.net",
                            FirstName = "Ерстенюк",
                            LastName = "Вікторія",
                            RegisteredAt = new DateTime(2025, 1, 21, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            Id = new Guid("a0f7b463-6eef-4a70-8444-789bbea23369"),
                            BirthDate = new DateTime(1999, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "bondarko@ukr.net",
                            FirstName = "Бондарко",
                            LastName = "Юрій",
                            RegisteredAt = new DateTime(2025, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified)
                        });
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.UserRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("CanCreate")
                        .HasColumnType("bit");

                    b.Property<bool>("CanDelete")
                        .HasColumnType("bit");

                    b.Property<bool>("CanRead")
                        .HasColumnType("bit");

                    b.Property<bool>("CanUpdate")
                        .HasColumnType("bit");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserRoles");

                    b.HasData(
                        new
                        {
                            Id = "SelfRegistered",
                            CanCreate = false,
                            CanDelete = false,
                            CanRead = false,
                            CanUpdate = false,
                            Description = "Самостійно зареєстрований користувач"
                        },
                        new
                        {
                            Id = "Employee",
                            CanCreate = true,
                            CanDelete = false,
                            CanRead = true,
                            CanUpdate = false,
                            Description = "Співробітник компанії"
                        },
                        new
                        {
                            Id = "Moderator",
                            CanCreate = false,
                            CanDelete = true,
                            CanRead = true,
                            CanUpdate = true,
                            Description = "Редактор контенту"
                        },
                        new
                        {
                            Id = "Administrator",
                            CanCreate = true,
                            CanDelete = true,
                            CanRead = true,
                            CanUpdate = true,
                            Description = "Системний адміністратор"
                        });
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.AccRates", b =>
                {
                    b.HasOne("Booking_WEB.Data.Entities.Realty", "Realty")
                        .WithOne("AccRates")
                        .HasForeignKey("Booking_WEB.Data.Entities.AccRates", "RealtyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Realty");
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.BookingItem", b =>
                {
                    b.HasOne("Booking_WEB.Data.Entities.Realty", "Realty")
                        .WithMany("BookingItems")
                        .HasForeignKey("RealtyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Booking_WEB.Data.Entities.UserAccess", "UserAccess")
                        .WithMany("BookingItems")
                        .HasForeignKey("UserAccessId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Realty");

                    b.Navigation("UserAccess");
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.Feedback", b =>
                {
                    b.HasOne("Booking_WEB.Data.Entities.Realty", "Realty")
                        .WithMany("Feedbacks")
                        .HasForeignKey("RealtyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Booking_WEB.Data.Entities.UserAccess", "UserAccess")
                        .WithMany("Feedbacks")
                        .HasForeignKey("UserAccessId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Realty");

                    b.Navigation("UserAccess");
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.ItemImage", b =>
                {
                    b.HasOne("Booking_WEB.Data.Entities.Realty", null)
                        .WithMany("Images")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Booking_WEB.Data.Entities.RealtyGroup", null)
                        .WithMany("Images")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.Realty", b =>
                {
                    b.HasOne("Booking_WEB.Data.Entities.City", "City")
                        .WithMany("Realties")
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Booking_WEB.Data.Entities.Country", "Country")
                        .WithMany("Realties")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Booking_WEB.Data.Entities.RealtyGroup", "RealtyGroup")
                        .WithMany("Realties")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("City");

                    b.Navigation("Country");

                    b.Navigation("RealtyGroup");
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.RealtyGroup", b =>
                {
                    b.HasOne("Booking_WEB.Data.Entities.RealtyGroup", "ParentGroup")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.Navigation("ParentGroup");
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.UserAccess", b =>
                {
                    b.HasOne("Booking_WEB.Data.Entities.UserRole", "UserRole")
                        .WithMany("UserAccesses")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Booking_WEB.Data.Entities.UserData", "UserData")
                        .WithMany("UserAccesses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserData");

                    b.Navigation("UserRole");
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.City", b =>
                {
                    b.Navigation("Realties");
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.Country", b =>
                {
                    b.Navigation("Realties");
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.Realty", b =>
                {
                    b.Navigation("AccRates");

                    b.Navigation("BookingItems");

                    b.Navigation("Feedbacks");

                    b.Navigation("Images");
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.RealtyGroup", b =>
                {
                    b.Navigation("Images");

                    b.Navigation("Realties");
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.UserAccess", b =>
                {
                    b.Navigation("BookingItems");

                    b.Navigation("Feedbacks");
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.UserData", b =>
                {
                    b.Navigation("UserAccesses");
                });

            modelBuilder.Entity("Booking_WEB.Data.Entities.UserRole", b =>
                {
                    b.Navigation("UserAccesses");
                });
#pragma warning restore 612, 618
        }
    }
}
