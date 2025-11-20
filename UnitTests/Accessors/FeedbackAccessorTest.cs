using Booking_WEB.Data;
using Booking_WEB.Data.DataAccessors;
using Booking_WEB.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace UnitTests.Accessors
{
    [TestClass]
    public class FeedbackAccessorTests
    {
        private DataContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DataContext(options);
        }

        private UserAccess CreateUser()
        {
            return new UserAccess
            {
                Id = Guid.NewGuid(),
                Login = "user1",
                Dk = "pass",
                Salt = "salt",
                RoleId = "SelfRegistered",
                UserData = new UserData { Id = Guid.NewGuid(), FirstName = "Test", LastName = "Test", Email = "1@gmail.com" }
            };
        }

        private Feedback CreateFeedback(Guid userId)
        {
            return new Feedback
            {
                Id = Guid.NewGuid(),
                UserAccessId = userId,
                Rate = 5,
                Text = "Nice!",
                CreatedAt = DateTime.UtcNow
            };
        }


        [TestMethod]
        public async Task ExistsAsync_ReturnsTrue_WhenNotDeleted()
        {
            var ctx = GetDbContext();
            var accessor = new FeedbackAccessor(ctx);

            var user = CreateUser();
            var fb = CreateFeedback(user.Id);
            ctx.UserAccesses.Add(user);
            ctx.Feedbacks.Add(fb);
            await ctx.SaveChangesAsync();

            var exists = await accessor.ExistsAsync(fb.Id);

            Assert.IsTrue(exists);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsFeedback_WithIncludes()
        {
            var ctx = GetDbContext();
            var accessor = new FeedbackAccessor(ctx);

            var user = CreateUser();
            var fb = CreateFeedback(user.Id);

            ctx.UserAccesses.Add(user);
            ctx.Feedbacks.Add(fb);
            await ctx.SaveChangesAsync();

            var loaded = await accessor.GetByIdAsync(fb.Id);

            Assert.IsNotNull(loaded);
            Assert.AreEqual(fb.Id, loaded.Id);
            Assert.IsNotNull(loaded.UserAccess);
            Assert.IsNotNull(loaded.UserAccess.UserData);
        }


        [TestMethod]
        public async Task CreateAsync_AddsFeedback()
        {
            var ctx = GetDbContext();
            var accessor = new FeedbackAccessor(ctx);

            var user = CreateUser();
            ctx.UserAccesses.Add(user);
            await ctx.SaveChangesAsync();

            var fb = CreateFeedback(user.Id);

            await accessor.CreateAsync(fb);

            var saved = await ctx.Feedbacks.FirstOrDefaultAsync(f => f.Id == fb.Id);
            Assert.IsNotNull(saved);
        }


        [TestMethod]
        public async Task CreateAsync_Throws_WhenUserAlreadyHasFeedback()
        {
            var ctx = GetDbContext();
            var accessor = new FeedbackAccessor(ctx);

            var user = CreateUser();
            var fb1 = CreateFeedback(user.Id);

            ctx.UserAccesses.Add(user);
            ctx.Feedbacks.Add(fb1);
            await ctx.SaveChangesAsync();

            var fb2 = CreateFeedback(user.Id);

            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await accessor.CreateAsync(fb2);
            });
        }
 
    }
}
