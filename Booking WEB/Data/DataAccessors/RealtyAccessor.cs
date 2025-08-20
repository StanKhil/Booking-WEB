using Booking_WEB.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking_WEB.Data.DataAccessors
{
    public class RealtyAccessor
    {
        private readonly DataContext _context;

        public RealtyAccessor(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null)
        {
            return await _context.Realties
                .AnyAsync(r => r.Slug == slug && r.DeletedAt == null && (!excludeId.HasValue || r.Id != excludeId.Value));
        }

        public async Task<Realty?> GetByIdAsync(Guid id, bool isEditable = false)
        {
            var query = _context.Realties.AsQueryable();
            if (!isEditable)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(r => r.Id == id && r.DeletedAt == null);
        }

        public async Task CreateAsync(Realty realty)
        {
            _context.Realties.Add(realty);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Realty realty)
        {
            _context.Realties.Update(realty);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Realty realty)
        {
            realty.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Realty>> GetAllAsync(bool isEditable = false)
        {
            IQueryable<Realty> query = _context.Realties
                .Include(r => r.City)
                .Include(r => r.Country)
                .Include(r => r.RealtyGroup);

            if (!isEditable)
                query = query.AsNoTracking();

            return await query
                .Where(r => r.DeletedAt == null)
                .ToListAsync();
        }

    }
}
