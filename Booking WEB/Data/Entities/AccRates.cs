using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Booking_WEB.Data.Entities
{
    public class AccRates
    {
        public Guid Id { get; set; }
        public Guid RealtyId { get; set; }
        public float AvgRate { get; set; }
        public int CountRate { get; set; }
        public DateTime LastRatedAt { get; set; }

        [JsonIgnore]
        public Realty Realty { get; set; } = null!;
    }
}
