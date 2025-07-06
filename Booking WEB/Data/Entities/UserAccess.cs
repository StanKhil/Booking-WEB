using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking_WEB.Data.Entities
{
    public class UserAccess
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string RoleId { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Salt { get; set; } = null!;
        public string Dk { get; set; } = null!; // Derived key by Rfc2898
        public UserData UserData { get; set; } = null!;
        public UserRole UserRole { get; set; } = null!;
        public List<BookingItem> BookingItems { get; set; } = [];
        public List<Feedback> Feedbacks { get; set; } = [];
    }
}
