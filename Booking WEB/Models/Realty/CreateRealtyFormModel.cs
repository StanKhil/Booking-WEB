using Microsoft.AspNetCore.Mvc;

namespace Booking_WEB.Models.Realty
{
    public class CreateRealtyFormModel
    {
        [FromForm(Name = "realty-name")]
        public String Name { get; set; } = null!;

        [FromForm(Name = "realty-description")]
        public String Description { get; set; } = null!;

        [FromForm(Name = "realty-slug")]
        public String Slug { get; set; } = null!;

        [FromForm(Name = "realty-price")]
        public decimal Price { get; set; }

        [FromForm(Name = "realty-country")]
        public String Country { get; set; } = null!;

        [FromForm(Name = "realty-city")]
        public String City { get; set; } = null!;

        [FromForm(Name = "realty-group")]
        public String Group { get; set; } = null!;

        [FromForm(Name = "realty-img")]
        public IFormFile Image { get; set; } = null!;
    }
}
