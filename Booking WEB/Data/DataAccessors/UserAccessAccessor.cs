using Booking_WEB.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Booking_WEB.Data.DataAccessors
{
    public class UserAccessAccessor
    {
        private readonly DataContext _context;
        private readonly ILogger<UserAccessAccessor> _logger;

        public UserAccessAccessor(DataContext context, ILogger<UserAccessAccessor> logger)
        {
            _context = context;
            _logger = logger;
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

        public async Task<bool> DeleteUserAsync(String userLogin)
        {
            UserAccess? ua = await _context
                .UserAccesses
                .Include(ua => ua.UserData)
                .FirstOrDefaultAsync(ua => ua.Login == userLogin);
            if (ua == null)
            {
                return false;
            }
            ua.UserData.FirstName = "";
            ua.UserData.LastName = "";
            ua.UserData.Email = "";
            ua.UserData.BirthDate = null;
            ua.UserData.DeletedAt = DateTime.UtcNow;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserLogin}", userLogin);
                return false;
            }
        }

        public async Task<UserAccess?> GerUserAccessByLoginAsync(string userLogin, bool isEditable = false)
        {
            IQueryable<UserAccess> source = _context.UserAccesses
                .Include(ua => ua.UserData)
                .Include(ua => ua.UserRole)
                .Include(ua => ua.BookingItems)
                .Include(ua => ua.Feedbacks);
            if (!isEditable)
                source = source.AsNoTracking();
            return await source.FirstOrDefaultAsync(ua => ua.Login == userLogin && ua.UserData.DeletedAt == null);
        }

        public async Task<UserAccess?> GetByIdAsync(Guid id, bool isEditable = false)
        {
            IQueryable<UserAccess> query = _context.UserAccesses
                .Include(ua => ua.UserData)
                .Include(ua => ua.UserRole);
            if (!isEditable)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync(ua => ua.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(UserAccess userAccess)
        {
            _context.UserAccesses.Update(userAccess);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserAccess>> GetAllAccesses(bool isEditable = false)
        {
            IQueryable<UserAccess> query = _context.UserAccesses
                .Include(ua => ua.UserData)
                .Include(ua => ua.UserRole);
            if (!isEditable)
                query = query.AsNoTracking();
            return await query.ToListAsync();
        }
    }
}
