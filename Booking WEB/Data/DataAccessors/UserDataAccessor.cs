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

        public async Task UpdateAsync(UserData user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id && u.DeletedAt == null);
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

        // При потребности вынеси в другой класс. Только надо будет исправить UserController в самом конце
        public async Task<Cards> CreateCardAsync(Cards card)
        {
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();
            return card;
        }
        public async Task<List<Cards>> GetCardsByUserIdAsync(Guid id, bool isEditable = false)
        {
            IQueryable<Cards> source = _context.Cards.Include(c => c.User);
            if(!isEditable) source = source.AsNoTracking();
            return await source.ToListAsync();
        }
    }
}
