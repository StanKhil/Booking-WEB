using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Booking_WEB.Data.Entities
{
    public class City
    {
        public Guid Id { get; set; }
        public String Name { get; set; } = null!;
        [JsonIgnore]
        public List<Realty> Realties { get; set; } = new List<Realty>();
    }
}
