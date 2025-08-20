using Booking_WEB.Data.Entities;

namespace Booking_WEB.Models.User
{
    public class UserProfilePageModel
    {
        public bool? IsPersonal { get; set; }
        public String? FirstName { get; set; }
        public String? LastName { get; set; }
        public String? Email { get; set; }
        public DateTime? Birthdate { get; set; }
        public DateTime? RegisteredAt { get; set; }
        public List<Cards> Cards { get; set; } = [];
    }
}
