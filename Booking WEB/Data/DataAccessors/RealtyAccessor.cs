using Booking_WEB.Data.Entities;
using Booking_WEB.Models.Realty;
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

        public async Task<List<Realty>> GetRealtiesByFilterAsync(SearchFiltersModel filters)
        {
            return await _context.Realties.Include(realty => realty.City).Include(realty => realty.RealtyGroup).Include(realty => realty.AccRates).Include(realty => realty.Images).AsNoTracking()
                .Where(realty => realty.DeletedAt == null && (realty.AccRates == null || realty.AccRates!.AvgRate >= filters.Rating) && realty.Price >= filters.Price && filters.Checkboxes.Contains(realty.RealtyGroup.Name))
                .ToListAsync();
        }
        //public async Task<List<Realty>> GetRealtiesByFilterAsync(String country, String city, String group, bool isEditable = false)
        //{
        //    IQueryable<Realty> query = _context.Realties
        //        .Include(r => r.City)
        //        .Include(r => r.RealtyGroup);

        //    if (!isEditable)
        //        query = query.AsNoTracking();

        //    if (!string.IsNullOrWhiteSpace(city))
        //    {
        //        query = query.Where(r => r.City.Name == city);
        //    }

        //    if (!string.IsNullOrWhiteSpace(group))
        //    {
        //        query = query.Where(r => r.RealtyGroup.Name == group);
        //    }

        //    return await query
        //        .Where(r => r.DeletedAt == null)
        //        .ToListAsync();
        //}

        public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId = null)
        {
            return await _context.Realties
                .AnyAsync(r => r.Slug == slug && r.DeletedAt == null && (!excludeId.HasValue || r.Id != excludeId.Value));
        }

        public async Task<Realty?> GetRealtyBySlugAsync(string slug, bool isEditable = false)
        {
            var query = _context.Realties.AsQueryable();
            if (!isEditable)
                query = query.AsNoTracking();

            return await query.Include(r => r.Images).Include(r => r.Feedbacks.Where(b => b.DeletedAt == null))
                .Include(r => r.City).Include(r => r.AccRates).Include(r => r.BookingItems.Where(b => b.DeletedAt == null)).FirstOrDefaultAsync(r => (r.Slug == slug || r.Id.ToString() == slug) && r.DeletedAt == null);
        }

        public async Task<Realty?> GetRealtyByIdAsync(Guid id, bool isEditable = false)
        {
            var query = _context.Realties.AsQueryable();
            if (!isEditable)
                query = query.AsNoTracking();
            return await query.Include(r => r.Images).Include(r => r.Feedbacks)
                .Include(r => r.City).FirstOrDefaultAsync(r => r.Id == id && r.DeletedAt == null);
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

        public async Task DeleteImagesByRealtySlug(string slug)
        {
            var realty = await _context.Realties
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Slug == slug && r.DeletedAt == null);
            if (realty != null)
            {
                _context.ItemImages.RemoveRange(realty.Images);
                await _context.SaveChangesAsync();
            }
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
                .ThenInclude(c => c.Country)
                .Include(r => r.RealtyGroup)
                .Include(r => r.AccRates)
                .Include(r => r.Feedbacks);

            if (!isEditable)
                query = query.AsNoTracking();

            return await query
                .Where(r => r.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<Guid> GetCityIdByNameAsync(String cityName, Guid countryId)
        {
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Name == cityName) ?? new City { Id = Guid.NewGuid(), Name = cityName };
            city.CountryId = countryId;
            if (!await _context.Cities.AnyAsync(c => c.Id == city.Id)) _context.Cities.Add(city);
            return city.Id;
        }

        public async Task<Guid> GetCountryIdByNameAsync(String countryName)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(c => c.Name == countryName) ?? new Country { Id = Guid.NewGuid(), Name = countryName };
            if (!await _context.Countries.AnyAsync(c => c.Id == country.Id)) _context.Countries.Add(country);
            return country.Id;
        }

        public async Task<Guid> GetGroupIdByNameAsync(String groupName)
        {
            var group = await _context.RealtyGroups.FirstOrDefaultAsync(g => g.Name == groupName) ?? new RealtyGroup { Id = Guid.NewGuid(), Name = groupName };
            if (!await _context.RealtyGroups.AnyAsync(g => g.Id == group.Id)) _context.RealtyGroups.Add(group);
            return group.Id;
        }

        public async Task<List<Realty>> GetRealtiesSortedByPrice()
        {
            return await _context.Realties
                .Where(r => r.DeletedAt == null)
                .OrderBy(r => r.Price)
                .ToListAsync();
        }

        public async Task<List<Realty>> GetRealtiesSortedByRating()
        {
            return await _context.Realties
                .Where(r => r.DeletedAt == null)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<List<Realty>> GetRealtiesByLowerRate(int rate)
        {
            return await _context.Realties
                .Where(r => r.DeletedAt == null && r.AccRates != null && r.AccRates.AvgRate >= rate)
                .ToListAsync();
        }

        public async Task<List<Realty>> GetRealtiesByPriceRange(decimal minPrice = 0, decimal maxPrice = 10000)
        {
            return await _context.Realties
                .Where(r => r.DeletedAt == null && r.Price >= minPrice && r.Price <= maxPrice)
                .ToListAsync();
        }
    }
}
