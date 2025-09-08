using Booking_WEB.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking_WEB.Models.Realty
{
    public class RealtyModel
    {
        // TO DO: Add more if required
        public String Name { get; set; } = null!;
        public String? Description { get; set; }
        public String? ImageUrl { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Price { get; set; }
        public AccRates? AccRates { get; set; }
        public City City { get; set; } = null!;
        public List<ItemImage> Images { get; set; } = [];
        public List<Feedback> Feedbacks { get; set; } = [];
    }
}
