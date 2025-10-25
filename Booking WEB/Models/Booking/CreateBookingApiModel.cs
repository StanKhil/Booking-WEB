namespace Booking_WEB.Models.Booking
{
    public class CreateBookingApiModel
    {
        public Guid RealtyId { get; set; }
        public Guid UserAccessId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
