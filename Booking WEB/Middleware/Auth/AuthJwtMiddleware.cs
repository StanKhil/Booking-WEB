using Booking_WEB.Services.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace Booking_WEB.Middleware.Auth
{
    public class AuthJwtMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthJwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IJwtService jwtService, ILogger<AuthJwtMiddleware> logger)
        {
            string? token = context.Request.Headers.Authorization
                                    .FirstOrDefault(h => h?.StartsWith("Bearer ") ?? false)?
                                    .Substring("Bearer ".Length)
                           ?? context.Request.Cookies["AuthToken"];

            //logger.LogInformation("JWT token from header/cookie: {token}", token);

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var (header, payload) = jwtService.DecodeJwt(token);
                    var payloadElement = (JsonElement)payload;

                    //logger.LogInformation("JWT payload: {payload}", payloadElement.ToString());

                    var claims = new List<Claim>();

                    if (payloadElement.TryGetProperty("FirstName", out var firstName))
                        claims.Add(new Claim(ClaimTypes.Name, firstName.GetString() ?? ""));

                    if (payloadElement.TryGetProperty("LastName", out var lastName))
                        claims.Add(new Claim(ClaimTypes.Surname, lastName.GetString() ?? ""));

                    if (payloadElement.TryGetProperty("Email", out var email))
                        claims.Add(new Claim(ClaimTypes.Email, email.GetString() ?? ""));

                    if (payloadElement.TryGetProperty("Role", out var role))
                        claims.Add(new Claim(ClaimTypes.Role, role.GetString() ?? ""));

                    if (payloadElement.TryGetProperty("Login", out var login))
                        claims.Add(new Claim(ClaimTypes.Sid, login.GetString() ?? ""));

                    context.User = new ClaimsPrincipal(
                        new ClaimsIdentity(claims, nameof(AuthJwtMiddleware))
                    );
                }
                catch (Exception e)
                {
                    logger.LogError("JWT decode error: {Message}", e.Message);
                }
            }

            await _next(context);
        }
    }

    public static class AuthJwtMiddlewareExtension
    {
        public static IApplicationBuilder UseAuthJwt(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthJwtMiddleware>();
        }
    }
}
