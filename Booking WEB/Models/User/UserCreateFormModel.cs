using Microsoft.AspNetCore.Mvc;

namespace Booking_WEB.Models.User
{
    public class UserCreateFormModel
    {
        [FromForm(Name = "user-first-name")]
        public String FirstName { get; set; } = null!;

        [FromForm(Name = "user-last-name")]
        public String LastName { get; set; } = null!;

        [FromForm(Name = "user-email")]
        public String Email { get; set; } = null!;

        [FromForm(Name = "user-login")]
        public String Login { get; set; } = null!;

        [FromForm(Name = "user-password")]
        public String Password { get; set; } = null!;
        [FromForm(Name = "user-birthdate")]
        public DateTime? Birthdate { get; set; }
        [FromForm(Name = "user-role")]
        public String RoleId { get; set; } = null!;
    }
}
