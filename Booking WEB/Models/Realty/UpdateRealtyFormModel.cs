using Microsoft.AspNetCore.Mvc;

namespace Booking_WEB.Models.Realty
{
    public class UpdateRealtyFormModel
    {
        [FromForm(Name = "realty-former-slug")]
        public String FormerSlug { get; set; } = null!;

        [FromForm(Name = "realty-name")]
        public String? Name { get; set; }
        [FromForm(Name = "realty-name")]
        public String? Description { get; set; }
        [FromForm(Name = "realty-slug")]
        public String? Slug { get; set; }
        [FromForm(Name = "realty-price")]
        public decimal Price { get; set; }

        [FromForm(Name = "realty-city")]
        public String? City { get; set; }
        [FromForm(Name = "realty-country")]
        public String? Country { get; set; }
        [FromForm(Name = "realty-group")]
        public String? Group { get; set; }

        [FromForm(Name = "realty-main-image")]
        public IFormFile? Image { get; set; }
        [FromForm(Name = "realty-secondary-images")]
        public List<IFormFile>? SecondaryImages { get; set; }
    }
}
