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
    public class AccessTokenAccessorTests
    {
        private DataContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DataContext(options);
        }

        [TestMethod]
        public async Task CreateAsync_ShouldAddAccessToken()
        {
            // Arrange
            var context = GetDbContext();
            var accessor = new AccessTokenAccessor(context);

            var token = new AccessToken
            {
                Jti = "test-jti",
                Sub = Guid.NewGuid(),
                Iat = "111",
                Exp = "222",
                Nbf = "333",
                Aud = "test-aud",
                Iss = "test-iss",
                UserAccess = new UserAccess
                {
                    Login = "testuser",
                    Dk = "123",
                    Salt = "salt",
                    RoleId = "SelfRegistered",
                    
                }
            };

            var created = await accessor.CreateAsync(token);

            // Assert
            Assert.IsNotNull(created);
            Assert.AreEqual("test-jti", created.Jti);

            var dbToken = await context.AccessTokens.FirstOrDefaultAsync(x => x.Jti == "test-jti");
            Assert.IsNotNull(dbToken);
            Assert.AreEqual("test-jti", dbToken.Jti);
        }
    }
}
