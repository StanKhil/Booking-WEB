namespace Booking_WEB.Models.Rest
{
    public class RestMeta
    {
        public long ServerTime { get; set; } = DateTime.Now.Ticks;
        public String ResourceName { get; set; } = null!;
        public String ResourceUrl { get; set; } = null!;
        public long Cache { get; set; }
        public String[] Manipulations { get; set; } = ["GET"];
        public Dictionary<String, String> Links { get; set; } = [];
        public String Method { get; set; } = "GET";
        public String DataType { get; set; } = null!;
    }
}
