namespace Booking_WEB.Data.Entities
{
    public class Cards
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Number { get; set; } = null!;
        public string CardholderName { get; set; } = null!;
        public DateTime ExpirationDate { get; set; }
        public UserData User { get; set; } = null!;
    }
}
