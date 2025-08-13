using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking_WEB.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace Booking_WEB.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Entities.UserData> Users { get; set; } = null!;
        public DbSet<Entities.UserRole> UserRoles { get; set; } = null!;
        public DbSet<Entities.UserAccess> UserAccesses { get; set; } = null!;
        public DbSet<Entities.AccessToken> AccessTokens { get; set; }

        public DbSet<Entities.RealtyGroup> RealtyGroups { get; set; }
        public DbSet<Entities.Realty> Realties { get; set; }
        public DbSet<Entities.ItemImage> ItemImages { get; set; }

        public DbSet<Entities.Country> Countries { get; set; } = null!;

        public DbSet<Entities.City> Cities { get; set; } = null!;

        public DbSet<Entities.BookingItem> BookingItems { get; set; } = null!;

        public DbSet<Entities.Feedback> Feedbacks { get; set; } = null!;

        public DbSet<Entities.AccRates> AccRates { get; set; } = null!;

        public DataContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.ItemImage>()
                .HasKey(i => new { i.ItemId, i.ImageUrl });

            modelBuilder.Entity<Entities.Realty>()
                .HasIndex(p => p.Slug)
                .IsUnique();

            modelBuilder.Entity<Entities.RealtyGroup>()
                .HasAlternateKey(pg => pg.Slug);

            modelBuilder.Entity<Entities.Realty>()
                .HasOne(r => r.RealtyGroup)
                .WithMany(rg => rg.Realties)
                .HasForeignKey(r => r.GroupId);

            modelBuilder.Entity<Entities.Realty>()
                .HasMany(r => r.Images)
                .WithOne()
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(p => p.ItemId);

            modelBuilder.Entity<Entities.RealtyGroup>()
                .HasMany(r => r.Images)
                .WithOne()
                .HasPrincipalKey(r => r.Id)
                .HasForeignKey(i => i.ItemId);

            modelBuilder.Entity<Entities.RealtyGroup>()
                .HasOne(rg => rg.ParentGroup)
                .WithMany()
                .HasForeignKey(rg => rg.ParentId);

            modelBuilder.Entity<UserAccess>().HasIndex(ua => ua.Login).IsUnique();
            modelBuilder.Entity<UserAccess>().HasOne(ua => ua.UserData).WithMany(ud => ud.UserAccesses).HasForeignKey(ua => ua.UserId);
            modelBuilder.Entity<UserAccess>().HasOne(ua => ua.UserRole).WithMany(ur => ur.UserAccesses).HasForeignKey(ua => ua.RoleId);

            modelBuilder.Entity<AccessToken>().HasKey(at => at.Jti);
            modelBuilder.Entity<AccessToken>().HasOne(at => at.UserAccess).WithMany().HasForeignKey(at => at.Sub);

            modelBuilder.ApplyConfiguration(new Configurations.RoleConfiguration());

            modelBuilder.Entity<Entities.Realty>()
                .HasOne(r => r.City)
                .WithMany(c => c.Realties)
                .HasForeignKey(r => r.CityId);

            modelBuilder.Entity<Entities.Realty>()
                .HasOne(r => r.Country)
                .WithMany(c => c.Realties)
                .HasForeignKey(r => r.CountryId);

            modelBuilder.Entity<Entities.BookingItem>()
                .HasOne(b => b.Realty)
                .WithMany(r => r.BookingItems)
                .HasForeignKey(b => b.RealtyId);

            modelBuilder.Entity<Entities.BookingItem>()
                .HasOne(b => b.UserAccess)
                .WithMany(ua => ua.BookingItems)
                .HasForeignKey(b => b.UserAccessId);

            modelBuilder.Entity<Entities.Feedback>()
                .HasOne(f => f.Realty)
                .WithMany(r => r.Feedbacks)
                .HasForeignKey(f => f.RealtyId);

            modelBuilder.Entity<Entities.Feedback>()
                .HasOne(f => f.UserAccess)
                .WithMany(ua => ua.Feedbacks)
                .HasForeignKey(f => f.UserAccessId);

            modelBuilder.Entity<Entities.AccRates>()
                .HasOne(ar => ar.Realty)
                .WithOne(r => r.AccRates);
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserData>().HasData(
                new UserData
                {
                    Id = Guid.Parse("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                    FirstName = "Палійчук",
                    LastName = "Яків",
                    Email = "jakiv@ukr.net",
                    BirthDate = new DateTime(1998, 3, 15),
                    RegisteredAt = new DateTime(2025, 3, 10)
                },
                new UserData
                {
                    Id = Guid.Parse("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                    FirstName = "Сторож",
                    LastName = "Чеслава",
                    Email = "storozh@ukr.net",
                    BirthDate = new DateTime(1999, 5, 11),
                    RegisteredAt = new DateTime(2025, 3, 15)
                },
                new UserData
                {
                    Id = Guid.Parse("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                    FirstName = "Дністрянський",
                    LastName = "Збоїслав",
                    Email = "dnistr@ukr.net",
                    BirthDate = new DateTime(1989, 7, 10),
                    RegisteredAt = new DateTime(2024, 8, 5)
                },
                new UserData
                {
                    Id = Guid.Parse("0d156354-89f1-4d58-a735-876b7add59d2"),
                    FirstName = "Гординська",
                    LastName = "Діна",
                    Email = "dina@ukr.net",
                    BirthDate = new DateTime(2005, 2, 15),
                    RegisteredAt = new DateTime(2024, 12, 20)
                },
                new UserData
                {
                    Id = Guid.Parse("a3c55a79-05ea-4053-ad3c-7301f3b7a7e2"),
                    FirstName = "Ромашко",
                    LastName = "Жадан",
                    Email = "romashko@ukr.net",
                    BirthDate = new DateTime(2005, 2, 15),
                    RegisteredAt = new DateTime(2024, 12, 20)
                },
                new UserData
                {
                    Id = Guid.Parse("eadb0b3b-523e-478b-88ee-b6cf57cbc05d"),
                    FirstName = "Ерстенюк",
                    LastName = "Вікторія",
                    Email = "erstenuk@ukr.net",
                    BirthDate = new DateTime(2001, 12, 21),
                    RegisteredAt = new DateTime(2025, 1, 21)
                },
                new UserData
                {
                    Id = Guid.Parse("a0f7b463-6eef-4a70-8444-789bbea23369"),
                    FirstName = "Бондарко",
                    LastName = "Юрій",
                    Email = "bondarko@ukr.net",
                    BirthDate = new DateTime(1999, 10, 21),
                    RegisteredAt = new DateTime(2025, 2, 2)
                }
            );

            modelBuilder.Entity<UserAccess>().HasData(
                new UserAccess
                {
                    Id = Guid.Parse("e29b6a1a-5bc7-4f42-9fa4-db25de342b42"),
                    UserId = Guid.Parse("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                    Login = "jakiv",
                    Salt = "Salt1",
                    Dk = "Salt1123",
                    RoleId = "SelfRegistered"
                },
                new UserAccess
                {
                    Id = Guid.Parse("fb4ad18c-d916-4708-be71-a9bbcf1eb806"),
                    UserId = Guid.Parse("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                    Login = "storozh",
                    Salt = "Salt2",
                    Dk = "Salt2123",
                    RoleId = "Employee"
                },
                new UserAccess
                {
                    Id = Guid.Parse("b31355b7-aa02-4b10-afda-eb9ec8294e78"),
                    UserId = Guid.Parse("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                    Login = "dnistr",
                    Salt = "Salt3",
                    Dk = "Salt3123",
                    RoleId = "SelfRegistered"
                },
                new UserAccess
                {
                    Id = Guid.Parse("92cd36b8-ea5a-4cbb-a232-268d942c97fd"),
                    UserId = Guid.Parse("0d156354-89f1-4d58-a735-876b7add59d2"),
                    Login = "dina",
                    Salt = "Salt4",
                    Dk = "Salt4123",
                    RoleId = "Employee"
                },
                new UserAccess
                {
                    Id = Guid.Parse("7a38a3aa-de9f-4519-bb48-eeb86c1efcdf"),
                    UserId = Guid.Parse("0d156354-89f1-4d58-a735-876b7add59d2"),
                    Login = "dina@ukr.net",
                    Salt = "Salt5",
                    Dk = "Salt5123",
                    RoleId = "Moderator"
                },
                new UserAccess
                {
                    Id = Guid.Parse("f1ea6b3f-0021-417b-95c8-f6cd333d7207"),
                    UserId = Guid.Parse("a3c55a79-05ea-4053-ad3c-7301f3b7a7e2"),
                    Login = "romashko",
                    Salt = "Salt6",
                    Dk = "Salt6123",
                    RoleId = "SelfRegistered"
                },
                new UserAccess
                {
                    Id = Guid.Parse("8806ca58-8daa-4576-92ba-797de42ffaa7"),
                    UserId = Guid.Parse("eadb0b3b-523e-478b-88ee-b6cf57cbc05d"),
                    Login = "erstenuk",
                    Salt = "Salt7",
                    Dk = "Salt7123",
                    RoleId = "Employee"
                },
                new UserAccess
                {
                    Id = Guid.Parse("97191468-a02f-4a78-927b-9ea660e9ea36"),
                    UserId = Guid.Parse("eadb0b3b-523e-478b-88ee-b6cf57cbc05d"),
                    Login = "erstenuk@ukr.net",
                    Salt = "Salt8",
                    Dk = "Salt8123",
                    RoleId = "Administrator"
                },
                new UserAccess
                {
                    Id = Guid.Parse("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"),
                    UserId = Guid.Parse("a0f7b463-6eef-4a70-8444-789bbea23369"),
                    Login = "bondarko",
                    Salt = "Salt9",
                    Dk = "Salt9123",
                    RoleId = "SelfRegistered"
                }
            );

            modelBuilder.Entity<RealtyGroup>().HasData(
                new RealtyGroup
                {
                    Id = Guid.Parse("f1ea6b3f-0021-417b-95c8-f6cd333d7207"),
                    ParentId = null,
                    Name = "Hotels",
                    Description = "Multi-room hotels",
                    Slug = "hotels",
                    ImageUrl = "hotel.jpg"
                },
                new RealtyGroup
                {
                    Id = Guid.Parse("8806ca58-8daa-4576-92ba-797de42ffaa7"),
                    ParentId = null,
                    Name = "Apartments",
                    Description = "Apartments",
                    Slug = "apartments",
                    ImageUrl = "apartment.jpg"
                },
                new RealtyGroup
                {
                    Id = Guid.Parse("97191468-a02f-4a78-927b-9ea660e9ea36"),
                    ParentId = null,
                    Name = "Houses",
                    Description = "Houses",
                    Slug = "houses",
                    ImageUrl = "house.jpg"
                },
                new RealtyGroup
                {
                    Id = Guid.Parse("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"),
                    ParentId = null,
                    Name = "Villas",
                    Description = "Villas",
                    Slug = "villas",
                    ImageUrl = "villa.jpg"
                }
                );

            modelBuilder.Entity<Country>().HasData(
                new Country
                {
                    Id = Guid.Parse("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                    Name = "Ukraine"
                },
                new Country
                {
                    Id = Guid.Parse("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                    Name = "Poland"
                }
            );

            modelBuilder.Entity<City>().HasData(
                new City
                {
                    Id = Guid.Parse("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                    Name = "Lviv",
                },
                new City
                {
                    Id = Guid.Parse("0d156354-89f1-4d58-a735-876b7add59d2"),
                    Name = "Krakow",
                }
            );



            SeedHotels(modelBuilder);
            SeedApartments(modelBuilder);
            SeedHouses(modelBuilder);
            SeedVillas(modelBuilder);
        }

        private void SeedHotels(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Realty>().HasData(
                new Realty
                {
                    Id = Guid.Parse("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d"),
                    GroupId = Guid.Parse("f1ea6b3f-0021-417b-95c8-f6cd333d7207"),
                    Name = "Готель \"Сонячний\"",
                    Description = "Готель \"Сонячний\" - це ідеальне місце для відпочинку на природі.",
                    Slug = "hotel-sunny",
                    ImageUrl = "hotel_sunny.jpg",
                    Price = 150.00m,
                    CityId = Guid.Parse("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                    CountryId = Guid.Parse("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d")
                },
                new Realty
                {
                    Id = Guid.Parse("bdf41cd9-c0f1-4349-8a44-4e67755d0415"),
                    GroupId = Guid.Parse("f1ea6b3f-0021-417b-95c8-f6cd333d7207"),
                    Name = "Готель \"Зоряний\"",
                    Description = "Готель \"Зоряний\" - це ідеальне місце для відпочинку на природі.",
                    Slug = "hotel-star",
                    ImageUrl = "hotel_star.jpg",
                    Price = 200.00m,
                    CityId = Guid.Parse("0d156354-89f1-4d58-a735-876b7add59d2"),
                    CountryId = Guid.Parse("bdf41cd9-c0f1-4349-8a44-4e67755d0415")
                },
                new Realty
                {
                    Id = Guid.Parse("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                    GroupId = Guid.Parse("f1ea6b3f-0021-417b-95c8-f6cd333d7207"),
                    Name = "Готель \"Лісовий\"",
                    Description = "Готель \"Лісовий\" - це ідеальне місце для відпочинку на природі.",
                    Slug = "hotel-forest",
                    ImageUrl = "hotel_forest.jpg",
                    Price = 250.00m,
                    CityId = Guid.Parse("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                    CountryId = Guid.Parse("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d")
                }
            );
        }

        private void SeedApartments(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Realty>().HasData(
                new Realty
                {
                    Id = Guid.Parse("0d156354-89f1-4d58-a735-876b7add59d2"),
                    GroupId = Guid.Parse("8806ca58-8daa-4576-92ba-797de42ffaa7"),
                    Name = "Квартира \"Центральна\"",
                    Description = "Квартира \"Центральна\" - це ідеальне місце для відпочинку в місті.",
                    Slug = "apartment-central",
                    ImageUrl = "apartment_central.jpg",
                    Price = 100.00m,
                    CityId = Guid.Parse("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                    CountryId = Guid.Parse("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d")
                },
                new Realty
                {
                    Id = Guid.Parse("a3c55a79-05ea-4053-ad3c-7301f3b7a7e2"),
                    GroupId = Guid.Parse("8806ca58-8daa-4576-92ba-797de42ffaa7"),
                    Name = "Квартира \"Люкс\"",
                    Description = "Квартира \"Люкс\" - це ідеальне місце для відпочинку, якщо ви не хочете виходити з дому.",
                    Slug = "apartment-luxury",
                    ImageUrl = "apartment_luxury.jpg",
                    Price = 150.00m,
                    CityId = Guid.Parse("0d156354-89f1-4d58-a735-876b7add59d2"),
                    CountryId = Guid.Parse("bdf41cd9-c0f1-4349-8a44-4e67755d0415")
                }
            );
        }

        private void SeedHouses(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Realty>().HasData(
                new Realty
                {
                    Id = Guid.Parse("eadb0b3b-523e-478b-88ee-b6cf57cbc05d"),
                    GroupId = Guid.Parse("97191468-a02f-4a78-927b-9ea660e9ea36"),
                    Name = "Будинок \"Садиба\"",
                    Description = "Будинок \"Садиба\" - це ідеальне місце для відпочинку з друзями.",
                    Slug = "house-mansion",
                    ImageUrl = "house_mansion.jpg",
                    Price = 300.00m,
                    CityId = Guid.Parse("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                    CountryId = Guid.Parse("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d")
                },
                new Realty
                {
                    Id = Guid.Parse("a0f7b463-6eef-4a70-8444-789bbea23369"),
                    GroupId = Guid.Parse("97191468-a02f-4a78-927b-9ea660e9ea36"),
                    Name = "Будинок \"Лісовий\"",
                    Description = "Будинок \"Лісовий\" - це ідеальне місце для відпочинку на природі.",
                    Slug = "house-forest",
                    ImageUrl = "house_forest.jpg",
                    Price = 350.00m,
                    CityId = Guid.Parse("0d156354-89f1-4d58-a735-876b7add59d2"),
                    CountryId = Guid.Parse("bdf41cd9-c0f1-4349-8a44-4e67755d0415")
                },
                new Realty
                {
                    Id = Guid.Parse("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"),
                    GroupId = Guid.Parse("97191468-a02f-4a78-927b-9ea660e9ea36"),
                    Name = "Будинок \"Гірський\"",
                    Description = "Будинок \"Гірський\" - це ідеальне місце для відпочинку в горах.",
                    Slug = "house-mountain",
                    ImageUrl = "house_mountain.jpg",
                    Price = 400.00m,
                    CityId = Guid.Parse("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                    CountryId = Guid.Parse("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d")
                }
            );
        }

        private void SeedVillas(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Realty>().HasData(
                new Realty
                {
                    Id = Guid.Parse("37DCC68E-B7E7-4B55-B04E-147C1A4126B7"),
                    GroupId = Guid.Parse("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"),
                    Name = "Вілла \"Сонячна\"",
                    Description = "Вілла \"Сонячна\" - це ідеальне місце для відпочинку на морі.",
                    Slug = "villa-sunny",
                    ImageUrl = "villa_sunny.jpg",
                    Price = 500.00m,
                    CityId = Guid.Parse("03767d46-aab3-4cc4-989c-a696a7fdd434"),
                    CountryId = Guid.Parse("7687bebd-e8a3-4b28-abc8-8fc9cc403a8d")
                },
                new Realty
                {
                    Id = Guid.Parse("D5E36E96-0314-4B7E-9CBF-D0FAE477AE36"),
                    GroupId = Guid.Parse("6a1d3de4-0d78-4d7d-8f6a-9e52694ff2ee"),
                    Name = "Вілла \"Лісова\"",
                    Description = "Вілла \"Лісова\" - це ідеальне місце для відпочинку на природі.",
                    Slug = "villa-forest",
                    ImageUrl = "villa_forest.jpg",
                    Price = 600.00m,
                    CityId = Guid.Parse("0d156354-89f1-4d58-a735-876b7add59d2"),
                    CountryId = Guid.Parse("bdf41cd9-c0f1-4349-8a44-4e67755d0415")
                }
            );
        }
    }
}
