namespace Booking_WEB.Services.Time
{
    public class SecTimeService : ITimeService
    {
        public long Timestamp()
        {
            return (DateTime.Now.Ticks - DateTime.UnixEpoch.Ticks) / 10000000;
        }
    }
}
