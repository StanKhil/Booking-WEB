using System.Text.Json.Serialization;

namespace Booking_WEB.Data.Entities
{
    public class AccessToken
    {
        public string Jti { get; set; } = null!;
        public Guid? Sub { get; set; }
        public string? Iat { get; set; }
        public string? Exp { get; set; }
        public string? Nbf { get; set; }
        public string? Aud { get; set; }
        public string? Iss { get; set; }

        [JsonIgnore]
        public UserAccess UserAccess { get; set; } = null!;
    }
}
