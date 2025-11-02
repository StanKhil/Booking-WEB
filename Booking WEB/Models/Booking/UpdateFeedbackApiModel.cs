namespace Booking_WEB.Models.Booking
{
    public class UpdateFeedbackApiModel
    {
        public Guid Id { get; set; }
        public Guid RealtyId { get; set; }
        public Guid UserAccessId { get; set; }
        public String Text { get; set; } = null!;
        public int Rate { get; set; }
    }
}
