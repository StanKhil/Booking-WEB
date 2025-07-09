using Microsoft.AspNetCore.Mvc;

namespace Booking_WEB.Models.User
{
    public class UserSignupFormModel
    {
        [FromForm(Name = "user-name")]
        public String UserName { get; set; }

        [FromForm(Name = "user-email")]
        public String UserEmail { get; set; }

        [FromForm(Name = "birthdate")]
        public DateTime? Birthdate { get; set; }

        [FromForm(Name = "user-login")]
        public String UserLogin { get; set; }

        [FromForm(Name = "user-password")]
        public String UserPassword { get; set; }

        [FromForm(Name = "user-repeat")]
        public String UserRepeat { get; set; }

        [FromForm(Name = "agree")]
        public bool Agree { get; set; } = false;
    }
}
