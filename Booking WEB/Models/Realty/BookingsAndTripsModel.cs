using Booking_WEB.Data.Entities;

namespace Booking_WEB.Models.Realty
{
    public class BookingsAndTripsModel
    {
        public IEnumerable<BookingItem> BookingItems { get; set; } = [];
    }
}
