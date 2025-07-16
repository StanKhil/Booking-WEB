namespace Booking_WEB.Models.User
{
    public class UserSignInPageModel
    {
        public UserSignInFormModel? FormModel { get; set; }
        public Dictionary<String, String>? FormErrors { get; set; }
    }
}
