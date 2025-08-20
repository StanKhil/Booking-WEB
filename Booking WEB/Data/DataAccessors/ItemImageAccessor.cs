using Booking_WEB.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking_WEB.Data.DataAccessors
{
    public class ItemImageAccessor(DataContext context)
    {
        private readonly DataContext _context = context;

        public async Task<List<ItemImage>> GetByItemIdAsync(Guid itemId, bool isEditable = false)
        {
            IQueryable<ItemImage> query = _context.ItemImages.Where(i => i.ItemId == itemId);

            if (!isEditable)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<List<string>> GetUrlsByItemIdAsync(Guid itemId)
        {
            return await _context.ItemImages
                .Where(i => i.ItemId == itemId)
                .Select(i => i.ImageUrl)
                .ToListAsync();
        }

        public async Task AddRangeAsync(Guid itemId, IEnumerable<string> urls)
        {
            foreach (var url in urls)
            {
                var image = new ItemImage
                {
                    ItemId = itemId,
                    ImageUrl = url,
                    Order = 0
                };
                _context.ItemImages.Add(image);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteByItemIdAsync(Guid itemId)
        {
            var images = await _context.ItemImages
                .Where(i => i.ItemId == itemId)
                .ToListAsync();

            if (images.Any())
            {
                _context.ItemImages.RemoveRange(images);
                await _context.SaveChangesAsync();
            }

            return images.Count;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
