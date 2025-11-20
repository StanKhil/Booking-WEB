using Booking_WEB.Data;
using Booking_WEB.Data.DataAccessors;
using Booking_WEB.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTests.Accessors
{
    [TestClass]
    public class BookingItemAccessorTests
    {
        private DataContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DataContext(options);
        }

        private BookingItem CreateSampleBookingItem(string login = "user1")
        {
            return new BookingItem
            {
                Id = Guid.NewGuid(),
                StartDate = new DateTime(2025, 1, 10),
                EndDate = new DateTime(2025, 1, 15),
                Realty = new Realty { Id = Guid.NewGuid(), Name = "Test realty" },
                UserAccess = new UserAccess { Login = login, Dk = "pass", Salt = "salt", RoleId = "SelfRegistered" }
            };
        }


        [TestMethod]
        public async Task ExistsAsync_ShouldReturnTrue_WhenExistsAndNotDeleted()
        {
            var context = GetDbContext();
            var accessor = new BookingItemAccessor(context);

            var item = CreateSampleBookingItem();
            context.BookingItems.Add(item);
            await context.SaveChangesAsync();

            var result = await accessor.ExistsAsync(item.Id);

            Assert.IsTrue(result);
        }


        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnItem_WhenExists()
        {
            var context = GetDbContext();
            var accessor = new BookingItemAccessor(context);

            var item = CreateSampleBookingItem();
            context.BookingItems.Add(item);
            await context.SaveChangesAsync();

            var result = await accessor.GetByIdAsync(item.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(item.Id, result.Id);
        }

        [TestMethod]
        public async Task GetBookingItemsByUserLoginAsync_ShouldReturnCorrectItems()
        {
            var context = GetDbContext();
            var accessor = new BookingItemAccessor(context);

            var item1 = CreateSampleBookingItem("stas");
            var item2 = CreateSampleBookingItem("stas");
            var item3 = CreateSampleBookingItem("stas");

            context.BookingItems.AddRange(item1, item2, item3);
            await context.SaveChangesAsync();

            var result = await accessor.GetBookingItemsByUserLoginAsync("stas");

            Assert.AreEqual(3, result.Count);
        }


        [TestMethod]
        public async Task CreateAsync_ShouldAddToDatabase()
        {
            var context = GetDbContext();
            var accessor = new BookingItemAccessor(context);

            var item = CreateSampleBookingItem();

            await accessor.CreateAsync(item);

            var saved = await context.BookingItems.FirstOrDefaultAsync(b => b.Id == item.Id);
            Assert.IsNotNull(saved);
        }


        [TestMethod]
        public async Task UpdateAsync_ShouldModifyExisting()
        {
            var context = GetDbContext();
            var accessor = new BookingItemAccessor(context);

            var item = CreateSampleBookingItem();
            context.BookingItems.Add(item);
            await context.SaveChangesAsync();

            item.EndDate = new DateTime(2025, 1, 20);
            await accessor.UpdateAsync(item);

            var updated = await context.BookingItems.FirstOrDefaultAsync(b => b.Id == item.Id);
            Assert.AreEqual(new DateTime(2025, 1, 20), updated.EndDate);
        }

        [TestMethod]
        public async Task SoftDeleteAsync_ShouldSetDeletedAt()
        {
            var context = GetDbContext();
            var accessor = new BookingItemAccessor(context);

            var item = CreateSampleBookingItem();
            context.BookingItems.Add(item);
            await context.SaveChangesAsync();

            await accessor.SoftDeleteAsync(item);

            var updated = await context.BookingItems.FirstOrDefaultAsync(b => b.Id == item.Id);
            Assert.IsNotNull(updated.DeletedAt);
        }

        [TestMethod]
        public async Task HasOverlapAsync_ShouldDetectOverlap()
        {
            var context = GetDbContext();
            var accessor = new BookingItemAccessor(context);

            var realtyId = Guid.NewGuid();
            var existing = new BookingItem
            {
                Id = Guid.NewGuid(),
                RealtyId = realtyId,
                StartDate = new DateTime(2025, 6, 10),
                EndDate = new DateTime(2025, 6, 20)
            };

            context.BookingItems.Add(existing);
            await context.SaveChangesAsync();

            var overlap = await accessor.HasOverlapAsync(
                realtyId,
                new DateTime(2025, 6, 15),
                new DateTime(2025, 6, 25)
            );

            Assert.IsTrue(overlap);
        }
    }
}
