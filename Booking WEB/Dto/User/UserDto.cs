namespace Booking_WEB.Dto.User
{
    public class UserDto
    {
        public String FirstName { get; set; } = null!;
        public String LastName { get; set; } = null!;
        public String Email { get; set; } = null!;
        public DateTime? BirthDate { get; set; }

        public String Login { get; set; } = null!;
        public String Password { get; set; } = null!;
        public String RoleId { get; set; } = "SelfRegistered";
    }
}
