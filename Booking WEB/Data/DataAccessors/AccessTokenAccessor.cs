using Booking_WEB.Data.Entities;

namespace Booking_WEB.Data.DataAccessors
{
    public class AccessTokenAccessor
    {
        private readonly DataContext _context;

        public AccessTokenAccessor(DataContext context)
        {
            _context = context;
        }

        public async Task<AccessToken?> CreateAsync(AccessToken accessToken)
        {
            _context.AccessTokens.Add(accessToken);
            await _context.SaveChangesAsync();
            return accessToken;
        }
    }
}
