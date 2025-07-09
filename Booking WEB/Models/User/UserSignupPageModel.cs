namespace Booking_WEB.Models.User
{
    public class UserSignupPageModel
    {
        public UserSignupFormModel? FormModel { get; set; }
        public Dictionary<String, String>? FormErrors { get; set; }
    }
}
