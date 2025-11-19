using Booking_WEB.Data;
using Booking_WEB.Data.DataAccessors;
using Booking_WEB.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Net.Mime.MediaTypeNames;

namespace UnitTests.Accessors
{
    [TestClass]
    public class RealtyAccessorTests
    {
        private DataContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new DataContext(options);
        }

        private RealtyAccessor GetAccessor(DataContext ctx) =>
            new RealtyAccessor(ctx);

        [TestMethod]
        public async Task SlugExistsAsync_ReturnsTrue_WhenSlugExists()
        {
            var ctx = GetDbContext();
            var accessor = GetAccessor(ctx);

            ctx.Realties.Add(new Realty
            {
                Id = Guid.NewGuid(),
                Slug = "test-slug",
                Name = "Test",
                CityId = Guid.NewGuid(),
                GroupId = Guid.NewGuid()
            });

            await ctx.SaveChangesAsync();

            var result = await accessor.SlugExistsAsync("test-slug");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task GetRealtyBySlugAsync_ReturnsEntity_WhenExists()
        {
            var ctx = GetDbContext();
            var accessor = GetAccessor(ctx);

            var id = Guid.NewGuid();

            ctx.Realties.Add(new Realty
            {
                Id = id,
                Slug = "my-slug",
                Name = "Test",
                City = new City { Id = Guid.NewGuid(), Name = "CityA" },
                CityId = Guid.NewGuid(),
                GroupId = Guid.NewGuid()
            });

            await ctx.SaveChangesAsync();

            var result = await accessor.GetRealtyBySlugAsync("my-slug");

            Assert.IsNotNull(result);
            Assert.AreEqual("my-slug", result.Slug);
        }


        [TestMethod]
        public async Task GetRealtyByIdAsync_ReturnsEntity_WhenExists()
        {
            var ctx = GetDbContext();
            var accessor = GetAccessor(ctx);

            var id = Guid.NewGuid();

            ctx.Realties.Add(new Realty
            {
                Id = id,
                Name = "Test",
                City = new City { Id = Guid.NewGuid(), Name = "CityA" },
                CityId = Guid.NewGuid(),
                GroupId = Guid.NewGuid()
            });
            await ctx.SaveChangesAsync();

            var result = await accessor.GetRealtyByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.Id);
        }


        [TestMethod]
        public async Task CreateAsync_AddsEntity()
        {
            var ctx = GetDbContext();
            var accessor = GetAccessor(ctx);

            var realty = new Realty
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                CityId = Guid.NewGuid(),
                GroupId = Guid.NewGuid()
            };

            await accessor.CreateAsync(realty);

            Assert.AreEqual(1, ctx.Realties.Count());
        }

        [TestMethod]
        public async Task UpdateAsync_UpdatesEntity()
        {
            var ctx = GetDbContext();
            var accessor = GetAccessor(ctx);

            var id = Guid.NewGuid();

            ctx.Realties.Add(new Realty
            {
                Id = id,
                Name = "OldName",
                CityId = Guid.NewGuid(),
                GroupId = Guid.NewGuid()
            });
            await ctx.SaveChangesAsync();

            var entity = ctx.Realties.First();
            entity.Name = "NewName";

            await accessor.UpdateAsync(entity);

            Assert.AreEqual("NewName", ctx.Realties.First().Name);
        }


        [TestMethod]
        public async Task DeleteImagesByRealtySlug_RemovesImages()
        {
            var ctx = GetDbContext();
            var accessor = GetAccessor(ctx);

            ctx.Realties.Add(new Realty
            {
                Id = Guid.NewGuid(),
                Slug = "test",
                Name = "R1",
                Images =
                {
                    new ItemImage { ItemId = Guid.NewGuid(), ImageUrl = "img1.jpg" },
                    new ItemImage { ItemId = Guid.NewGuid(), ImageUrl = "img2.jpg" }
                },
                CityId = Guid.NewGuid(),
                GroupId = Guid.NewGuid()
            });

            await ctx.SaveChangesAsync();

            await accessor.DeleteImagesByRealtySlug("test");

            Assert.AreEqual(0, ctx.ItemImages.Count());
        }


        [TestMethod]
        public async Task SoftDeleteAsync_SetsDeletedAt()
        {
            var ctx = GetDbContext();
            var accessor = GetAccessor(ctx);

            var entity = new Realty
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                CityId = Guid.NewGuid(),
                GroupId = Guid.NewGuid()
            };

            ctx.Realties.Add(entity);
            await ctx.SaveChangesAsync();

            await accessor.SoftDeleteAsync(entity);

            Assert.IsNotNull(entity.DeletedAt);
        }

        [TestMethod]
        public async Task GetAllAsync_ReturnsOnlyNotDeleted()
        {
            var ctx = GetDbContext();
            var accessor = GetAccessor(ctx);

            ctx.Realties.Add(new Realty
            {
                Id = Guid.NewGuid(),
                Name = "A",
                CityId = Guid.NewGuid(),
                GroupId = Guid.NewGuid()
            });

            ctx.Realties.Add(new Realty
            {
                Id = Guid.NewGuid(),
                Name = "B",
                DeletedAt = DateTime.UtcNow,
                CityId = Guid.NewGuid(),
                GroupId = Guid.NewGuid()
            });

            await ctx.SaveChangesAsync();

            var result = await accessor.GetAllAsync();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetRealtiesByFilterAsync_ReturnsFiltered()
        {
            var ctx = GetDbContext();
            var accessor = GetAccessor(ctx);

            var city = new City { Id = Guid.NewGuid(), Name = "Kyiv" };
            var group = new RealtyGroup
            {
                Id = Guid.NewGuid(),
                Name = "House",
                Description = "Test group",
                Slug = "house",
                ImageUrl = "img.png"
            };

            ctx.Cities.Add(city);
            ctx.RealtyGroups.Add(group);

            ctx.Realties.Add(new Realty
            {
                Id = Guid.NewGuid(),
                Name = "R1",
                City = city,
                CityId = city.Id,
                RealtyGroup = group,
                GroupId = group.Id
            });

            await ctx.SaveChangesAsync();

            var result = await accessor.GetRealtiesByFilterAsync("Ukraine", "Kyiv", "House");

            Assert.AreEqual(1, result.Count);
        }



        [TestMethod]
        public async Task GetRealtiesSortedByPrice_ReturnsSortedAsc()
        {
            var ctx = GetDbContext();
            var accessor = GetAccessor(ctx);

            ctx.Realties.Add(new Realty
            {
                Id = Guid.NewGuid(),
                Name = "A",
                Price = 200,
                CityId = Guid.NewGuid(),
                GroupId = Guid.NewGuid()
            });

            ctx.Realties.Add(new Realty
            {
                Id = Guid.NewGuid(),
                Name = "B",
                Price = 100,
                CityId = Guid.NewGuid(),
                GroupId = Guid.NewGuid()
            });

            await ctx.SaveChangesAsync();

            var result = await accessor.GetRealtiesSortedByPrice();

            Assert.AreEqual("B", result.First().Name);
        }


        [TestMethod]
        public async Task GetRealtiesSortedByRating_ReturnsSortedByName()
        {
            var ctx = GetDbContext();
            var accessor = GetAccessor(ctx);

            ctx.Realties.Add(new Realty { Id = Guid.NewGuid(), Name = "Z", CityId = Guid.NewGuid(), GroupId = Guid.NewGuid() });
            ctx.Realties.Add(new Realty { Id = Guid.NewGuid(), Name = "A", CityId = Guid.NewGuid(), GroupId = Guid.NewGuid() });

            await ctx.SaveChangesAsync();

            var result = await accessor.GetRealtiesSortedByRating();

            Assert.AreEqual("A", result.First().Name);
        }


        [TestMethod]
        public async Task GetRealtiesByLowerRate_ReturnsCorrect()
        {
            var ctx = GetDbContext();
            var accessor = GetAccessor(ctx);

            ctx.Realties.Add(new Realty
            {
                Id = Guid.NewGuid(),
                Name = "AAA",
                AccRates = new AccRates { AvgRate = 5 },
                CityId = Guid.NewGuid(),
                GroupId = Guid.NewGuid()
            });

            await ctx.SaveChangesAsync();

            var result = await accessor.GetRealtiesByLowerRate(3);

            Assert.AreEqual(1, result.Count);
        }


        [TestMethod]
        public async Task GetRealtiesByPriceRange_ReturnsCorrect()
        {
            var ctx = GetDbContext();
            var accessor = GetAccessor(ctx);

            ctx.Realties.Add(new Realty
            {
                Id = Guid.NewGuid(),
                Name = "Cheap",
                Price = 100,
                CityId = Guid.NewGuid(),
                GroupId = Guid.NewGuid()
            });

            ctx.Realties.Add(new Realty
            {
                Id = Guid.NewGuid(),
                Name = "Expensive",
                Price = 5000,
                CityId = Guid.NewGuid(),
                GroupId = Guid.NewGuid()
            });

            await ctx.SaveChangesAsync();

            var result = await accessor.GetRealtiesByPriceRange(0, 1000);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Cheap", result.First().Name);
        }
    }
}
