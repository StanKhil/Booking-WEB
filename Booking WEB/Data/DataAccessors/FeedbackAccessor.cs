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
            var ua = await _context.UserAccesses.FirstOrDefaultAsync(ua => ua.Id == feedback.UserAccessId);

            if(ua?.Feedbacks.Count() > 0)
            {
                throw new Exception("User has already submitted feedback for this realty.");
            }

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Feedback feedback)
        {
            await _context.Feedbacks
                .Where(f => f.Id == feedback.Id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(f => f.Rate, feedback.Rate)
                    .SetProperty(f => f.Text, feedback.Text)
                    .SetProperty(f => f.UpdatedAt, DateTime.UtcNow)
                );
        }


        // changed on hard delete for trigger
        public async Task SoftDeleteAsync(Feedback feedback)
        {
            await _context.Feedbacks
                .Where(f => f.Id == feedback.Id)
                .ExecuteDeleteAsync();
        }

    }
}
