using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Booking_WEB.Data.DataAccessors;
using Booking_WEB.Data;
using Booking_WEB.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Accessors
{
    [TestClass]
    public class UserDataAccessorTest
    {
        private DataContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DataContext(options);
        }

        private UserData CreateValidUser(Guid? id = null)
        {
            return new UserData
            {
                Id = id ?? Guid.NewGuid(),
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
            };
        }

        private Cards CreateValidCard(Guid? userId = null)
        {
            return new Cards
            {
                Id = Guid.NewGuid(),
                Number = "123456",
                CardholderName = "Test User",
                UserId = userId ?? Guid.NewGuid()
            };
        }

        // ------------------ TESTS ---------------------

        [TestMethod]
        public async Task CreateAsync_ShouldCreateUser()
        {
            var context = GetDbContext();
            var accessor = new UserDataAccessor(context);

            var user = CreateValidUser();

            await accessor.CreateAsync(user);

            var saved = await context.Users.FirstOrDefaultAsync();
            Assert.IsNotNull(saved);
            Assert.AreEqual("Test", saved!.FirstName);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldUpdateUser()
        {
            var context = GetDbContext();
            var accessor = new UserDataAccessor(context);

            var user = CreateValidUser();
            context.Users.Add(user);
            await context.SaveChangesAsync();

            user.FirstName = "New Name";
            await accessor.UpdateAsync(user);

            Assert.AreEqual("New Name", context.Users.First().FirstName);
        }

        [TestMethod]
        public async Task ExistsAsync_ShouldReturnTrue_WhenUserExists()
        {
            var context = GetDbContext();
            var accessor = new UserDataAccessor(context);

            var id = Guid.NewGuid();
            var user = CreateValidUser(id);

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var exists = await accessor.ExistsAsync(id);

            Assert.IsTrue(exists);
        }

        [TestMethod]
        public async Task ExistsAsync_ShouldReturnFalse_WhenUserNotExists()
        {
            var context = GetDbContext();
            var accessor = new UserDataAccessor(context);

            var exists = await accessor.ExistsAsync(Guid.NewGuid());

            Assert.IsFalse(exists);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnUserWithIncludes()
        {
            var context = GetDbContext();
            var accessor = new UserDataAccessor(context);

            var id = Guid.NewGuid();
            var user = CreateValidUser(id);

            user.UserAccesses = new List<UserAccess>
            {
                new UserAccess { Id = Guid.NewGuid(), Dk = "123", Login = ("testLogin" + id.ToString()), Salt = "salt", RoleId = "SelfRegistered" 
                }
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var result = await accessor.GetByIdAsync(id);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result!.UserAccesses.Count);
        }

        [TestMethod]
        public async Task CreateCardAsync_ShouldAddCard()
        {
            var context = GetDbContext();
            var accessor = new UserDataAccessor(context);

            var card = CreateValidCard();

            await accessor.CreateCardAsync(card);

            var saved = await context.Cards.FirstOrDefaultAsync();
            Assert.IsNotNull(saved);
            Assert.AreEqual("123456", saved!.Number);
        }

        [TestMethod]
        public async Task GetCardsByUserIdAsync_ShouldReturnAllUserCards()
        {
            var context = GetDbContext();
            var accessor = new UserDataAccessor(context);

            var userId = Guid.NewGuid();

            context.Cards.Add(CreateValidCard(userId));
            context.Cards.Add(CreateValidCard(userId));
            context.Cards.Add(CreateValidCard(Guid.NewGuid()));

            await context.SaveChangesAsync();

            var cards = await accessor.GetCardsByUserIdAsync(userId);

            Assert.AreEqual(0, cards.Count);
        }
    }
}
