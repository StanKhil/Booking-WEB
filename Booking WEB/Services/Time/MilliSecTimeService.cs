namespace Booking_WEB.Services.Time
{
    public class MilliSecTimeService : ITimeService
    {
        public long Timestamp()
        {
            return (DateTime.Now.Ticks - DateTime.UnixEpoch.Ticks) / (long)1e4;
        }
    }
}
