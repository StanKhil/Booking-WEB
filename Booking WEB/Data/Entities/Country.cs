using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking_WEB.Data.Entities
{
    public class Country
    {
        public Guid Id { get; set; }
        public String Name { get; set; } = null!;

        public List<Realty> Realties { get; set; } = new List<Realty>();
    }
}
