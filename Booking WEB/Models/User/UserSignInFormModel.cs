using Microsoft.AspNetCore.Mvc;

namespace Booking_WEB.Models.User
{
    public class UserSignInFormModel
    {
        [FromForm(Name = "user-login")]
        public String UserLogin { get; set; } = null!;

        [FromForm(Name = "user-password")]
        public String UserPassword { get; set; } = null!;
    }
}
