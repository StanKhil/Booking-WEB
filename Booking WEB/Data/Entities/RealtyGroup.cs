using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Booking_WEB.Data.Entities
{
    public class RealtyGroup
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public String Name { get; set; } = null!;
        public String Description { get; set; } = null!;
        public String Slug { get; set; } = null!;  
        public String ImageUrl { get; set; } = null!;
        public DateTime? DeletedAt { get; set; }


        public RealtyGroup? ParentGroup { get; set; }
        [JsonIgnore]
        public List<Realty> Realties { get; set; } = [];
        public List<ItemImage> Images { get; set; } = [];
    }
}
