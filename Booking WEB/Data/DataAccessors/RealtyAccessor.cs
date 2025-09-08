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

        public async Task<Realty?> GetRealtyBySlugAsync(string slug, bool isEditable = false)
        {
            var query = _context.Realties.AsQueryable();
            if (!isEditable)
                query = query.AsNoTracking();

            return await query.Include(r => r.Images).Include(r => r.Feedbacks)
                .Include(r => r.City).FirstOrDefaultAsync(r => (r.Slug == slug || r.Id.ToString() == slug) && r.DeletedAt == null);
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
                .Include(r => r.RealtyGroup);

            if (!isEditable)
                query = query.AsNoTracking();

            return await query
                .Where(r => r.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<Guid> GetCityIdByNameAsync(String cityName)
        {
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Name == cityName) ?? new City { Id = Guid.NewGuid(), Name = cityName };
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

        public async Task<List<Realty>> GetRealtiesByFilterAsync(String country, String city, String group, bool isEditable = false)
        {
            IQueryable<Realty> query = _context.Realties
                .Include(r => r.City)
                .Include(r => r.RealtyGroup);

            if (!isEditable)
                query = query.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(r => r.City.Name == city);
            }

            if (!string.IsNullOrWhiteSpace(group))
            {
                query = query.Where(r => r.RealtyGroup.Name == group);
            }

            return await query
                .Where(r => r.DeletedAt == null)
                .ToListAsync();
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
    }
}
