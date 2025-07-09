using Booking_WEB.Services.Time;

namespace Booking_WEB.Services.Random
{
    public class DefaultRandomService : IRandomService
    {
        private readonly System.Random random;
        public DefaultRandomService(ITimeService timeService) 
        {
            random = new System.Random((int)timeService.Timestamp());
        }
        public string Otp(int length)
        {
            return String.Join("", 
                Enumerable.Range(0, length).Select(_ => random.Next(10)));
        }
    }
}
