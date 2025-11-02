namespace Booking_WEB.Models.Feedback
{
    public class UpdateBookingApiModel
    {
        public Guid Id { get; set; }
        public Guid RealtyId { get; set; }
        public Guid UserAccessId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
