namespace Booking_WEB.Models.Feedback
{
    public class CreateFeedbackApiModel
    {
        public Guid RealtyId { get; set; }
        public Guid UserAccessId { get; set; }
        public String Text { get; set; } = null!;
        public int Rate { get; set; }
    }
}
