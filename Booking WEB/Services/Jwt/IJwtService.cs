namespace Booking_WEB.Services.Jwt
{
    public interface IJwtService
    {
        public abstract string EncodeJwt(object payload, object? header = null, string? secret = null);
        public abstract (object, object) DecodeJwt(string jwt, string? secret = null);
    }
}
