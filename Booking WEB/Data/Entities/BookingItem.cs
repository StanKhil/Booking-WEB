using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking_WEB.Data.Entities
{
    public class BookingItem
    {
        public Guid Id { get; set; }
        public Guid RealtyId { get; set; }
        public Guid UserAccessId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Realty Realty { get; set; } = null!;
        public UserAccess UserAccess { get; set; } = null!;
    }
}
