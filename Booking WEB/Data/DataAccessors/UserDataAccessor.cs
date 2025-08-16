using Booking_WEB.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking_WEB.Data.DataAccessors
{
    public class UserDataAccessor
    {
        private readonly DataContext _context;

        public UserDataAccessor(DataContext context)
        {
            _context = context;
        }

        public async Task<UserData> CreateAsync(UserData user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<UserData?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.UserAccesses)
                .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);
        }
    }
}
