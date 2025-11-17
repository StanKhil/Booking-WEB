using Booking_WEB.Data;
using Booking_WEB.Data.DataAccessors;
using Booking_WEB.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTests.Accessors
{
    [TestClass]
    public class UserAccessAccessorTests
    {
        private DataContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DataContext(options);
        }

        private UserAccess CreateTestUser(string login = "testuser")
        {
            return new UserAccess
            {
                Id = Guid.NewGuid(),
                Login = login,
                Dk = "derived key",
                Salt = "salt",
                UserData = new UserData
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@test.com",
                    BirthDate = new DateTime(1990, 1, 1)
                },
                UserRole = new UserRole { Id = "SelfRegistered", Description = "!" }
            };
        }

        [TestMethod]
        public async Task LoginExistsAsync_ShouldReturnTrueIfLoginExists()
        {
            var context = GetDbContext();
            var accessor = new UserAccessAccessor(context, NullLogger<UserAccessAccessor>.Instance);

            var user = CreateTestUser();
            context.UserAccesses.Add(user);
            await context.SaveChangesAsync();

            var exists = await accessor.LoginExistsAsync(user.Login);

            Assert.IsTrue(exists);
        }

        [TestMethod]
        public async Task CreateAsync_ShouldAddUserAccess()
        {
            var context = GetDbContext();
            var accessor = new UserAccessAccessor(context, NullLogger<UserAccessAccessor>.Instance);

            var user = CreateTestUser();

            var result = await accessor.CreateAsync(user);

            Assert.IsNotNull(result);
            Assert.AreEqual(user.Login, result.Login);
            Assert.AreEqual(1, context.UserAccesses.Count());
        }

        [TestMethod]
        public async Task DeleteUserAsync_ShouldMarkUserAsDeleted()
        {
            var context = GetDbContext();
            var accessor = new UserAccessAccessor(context, NullLogger<UserAccessAccessor>.Instance);

            var user = CreateTestUser();
            context.UserAccesses.Add(user);
            await context.SaveChangesAsync();

            var deleted = await accessor.DeleteUserAsync(user.Login);

            Assert.IsTrue(deleted);
            var updatedUser = await context.UserAccesses.Include(u => u.UserData)
                .FirstAsync(u => u.Login == user.Login);
            Assert.AreEqual("", updatedUser.UserData.FirstName);
            Assert.AreEqual("", updatedUser.UserData.LastName);
            Assert.AreEqual("", updatedUser.UserData.Email);
            Assert.IsNotNull(updatedUser.UserData.DeletedAt);
        }

        [TestMethod]
        public async Task GerUserAccessByLoginAsync_ShouldReturnUser_WhenNotDeleted()
        {
            var context = GetDbContext();
            var accessor = new UserAccessAccessor(context, NullLogger<UserAccessAccessor>.Instance);

            var user = CreateTestUser();
            context.UserAccesses.Add(user);
            await context.SaveChangesAsync();

            var result = await accessor.GerUserAccessByLoginAsync(user.Login);

            Assert.IsNotNull(result);
            Assert.AreEqual(user.Login, result.Login);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnUserById()
        {
            var context = GetDbContext();
            var accessor = new UserAccessAccessor(context, NullLogger<UserAccessAccessor>.Instance);

            var user = CreateTestUser();
            context.UserAccesses.Add(user);
            await context.SaveChangesAsync();

            var result = await accessor.GetByIdAsync(user.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.Id);
        }
    }
}
