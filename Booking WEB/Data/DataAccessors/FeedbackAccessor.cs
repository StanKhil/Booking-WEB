using Booking_WEB.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking_WEB.Data.DataAccessors
{
    public class FeedbackAccessor(DataContext context)
    {
        private readonly DataContext _context = context ?? throw new Exception("context not found");

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Feedbacks.AnyAsync(f => f.Id == id && f.DeletedAt == null);
        }

        public async Task<Feedback?> GetByIdAsync(Guid id, bool isEditable = false)
        {
            var query = _context.Feedbacks.Include(f => f.UserAccess).ThenInclude(ua => ua.UserData).AsQueryable();
            if (!isEditable)
                query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync(f => f.Id == id && f.DeletedAt == null);
        }

        public async Task CreateAsync(Feedback feedback)
        {
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Feedback feedback)
        {
            _context.Feedbacks.Update(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Feedback feedback)
        {
            await _context.Feedbacks
                .Where(f => f.Id == feedback.Id)
                .ExecuteUpdateAsync(f => f
                    .SetProperty(fb => fb.DeletedAt, DateTime.UtcNow));
        }

    }
}
