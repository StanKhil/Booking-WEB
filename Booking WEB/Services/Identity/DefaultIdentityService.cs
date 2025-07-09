using Booking_WEB.Services.Time;

namespace Booking_WEB.Services.Identity
{
    public class DefaultIdentityService : IIdentityService
    {
        private static MilliSecTimeService _milliSecTimeService = new();
        private static int counter = 0;
        private static string prev = null;
        public string Identity()
        {
            string timestamp = _milliSecTimeService.Timestamp().ToString();

            if (prev == null) timestamp += "0";
            else timestamp += (++counter).ToString();

            prev = timestamp;

            char[] charArray = timestamp.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
