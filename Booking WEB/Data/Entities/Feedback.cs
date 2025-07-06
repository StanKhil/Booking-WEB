using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking_WEB.Data.Entities
{
    public class Feedback
    {
        public Guid Id { get; set; }
        public Guid RealtyId { get; set; }
        public Guid UserAccessId { get; set; }
        public String Text { get; set; } = null!;
        public int Rate { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Realty Realty { get; set; } = null!;
        public UserAccess UserAccess { get; set; } = null!;
    }
}
