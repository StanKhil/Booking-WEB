using Booking_WEB.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking_WEB.Data.DataAccessors
{
    public class BookingItemAccessor(DataContext context)
    {
        private readonly DataContext _context = context ?? throw new Exception("context not found");
        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.BookingItems.AnyAsync(bi => bi.Id == id && bi.DeletedAt == null);
        }

        public async Task<BookingItem?> GetByIdAsync(Guid id, bool isEditable = false)
        {
            var query = _context.BookingItems.AsQueryable();
            if (!isEditable)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync(bi => bi.Id == id && bi.DeletedAt == null);
        }
        public async Task<List<BookingItem>> GetBookingItemsByUserLoginAsync(string userLogin)
        {
            return await _context.BookingItems.AsNoTracking().Include(bi => bi.Realty)
                .Include(bi => bi.UserAccess).Where(bi => bi.UserAccess.Login == userLogin).ToListAsync();
        }
        
        public async Task CreateAsync(BookingItem bookingItem)
        {
            _context.BookingItems.Add(bookingItem);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateAsync(BookingItem bookingItem)
        {
            _context.BookingItems.Update(bookingItem);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(BookingItem bookingItem)
        {
            bookingItem.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasOverlapAsync(Guid realtyId, DateTime startDate, DateTime endDate, Guid? excludeId = null)
        {
            return await _context.BookingItems.AnyAsync(b =>
                b.RealtyId == realtyId &&
                b.DeletedAt == null &&
                (excludeId == null || b.Id != excludeId) &&
                (
                    (startDate >= b.StartDate && startDate < b.EndDate) || 
                    (endDate > b.StartDate && endDate <= b.EndDate) ||    
                    (startDate <= b.StartDate && endDate >= b.EndDate)   
                ));
        }
    }
}
