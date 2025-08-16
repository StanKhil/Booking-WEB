using Booking_WEB.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Booking_WEB.Data.DataAccessors
{
    public class UserAccessAccessor
    {
        private readonly DataContext _context;

        public UserAccessAccessor(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> LoginExistsAsync(string login)
        {
            return await _context.UserAccesses.AnyAsync(a => a.Login == login);
        }

        public async Task<UserAccess> CreateAsync(UserAccess access)
        {
            _context.UserAccesses.Add(access);
            await _context.SaveChangesAsync();
            return access;
        }

        public async Task<UserAccess?> GerUserAccessByLoginAsync(String userLogin, bool isEditable = false)
        {
            IQueryable<UserAccess> source = _context.UserAccesses
                .Include(ua => ua.UserData)
                .Include(ua => ua.UserRole);
            if (!isEditable)
                source = source.AsNoTracking();
            return await source.FirstOrDefaultAsync(ua => ua.Login == userLogin);
        }
    }
}
