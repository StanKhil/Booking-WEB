using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace Booking_WEB.Services.Jwt
{
    public class JwtService : IJwtService
    {
        const string defaultSecret = "JwtService";

        public (object, object) DecodeJwt(string jwt, string? secret = null)
        {
            int lastDotIndex = jwt.LastIndexOf('.');
            if(lastDotIndex == -1)
            {
                throw new Exception("Invalid format: dot was not found");
            }

            secret ??= defaultSecret;
            string signature = jwt[(lastDotIndex + 1)..];
            string openPart = jwt[..lastDotIndex]; // Header + Payload

            string controlSign = Sign(openPart, secret);
            if(controlSign != signature)
            {
                throw new Exception("Invalid signature");
            }
            string[] parts = openPart.Split('.');
            if(parts.Length != 2)
            {
                throw new Exception("Invalid format: dot was not found in openPart");
            }

            var header = JsonSerializer.Deserialize<JsonElement>(Encoding.UTF8.GetString(Base64UrlTextEncoder.Decode(parts[0])));
            var payload = JsonSerializer.Deserialize<JsonElement>(Encoding.UTF8.GetString(Base64UrlTextEncoder.Decode(parts[1])));
            return (header, payload);
        }

        public string EncodeJwt(object payload, object? header = null, string? secret = null)
        {
            secret ??= defaultSecret;
            header ??= new
            {
                alg = "HS256",
                typ = "JWT"
            };
            string openPart = Base64UrlTextEncoder.Encode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(header))) + "."
                + Base64UrlTextEncoder.Encode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)));
            string signature = Sign(openPart, secret);

            return openPart + "." + signature;
        }

        private string Sign(string openPart, string? secret = null)
        {
            secret ??= defaultSecret;
            return Base64UrlTextEncoder.Encode(System.Security.Cryptography.HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(openPart)));
        }
    }
}
