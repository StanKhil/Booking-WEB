using Booking_WEB.Data.Entities;
using System.Globalization;
using System.Text.Json;
using System.Security.Claims;

namespace Booking_WEB.Middleware.Auth
{
    public class AuthSessionMiddleware
    {
        private readonly RequestDelegate _next;
        public AuthSessionMiddleware(RequestDelegate next)
        {
            _next = next; 
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Query.ContainsKey("logout"))
            {
                context.Session.Remove("userAccess");
                context.Response.Redirect(context.Request.Path);
                return;
            }
            else if (context.Session.Keys.Contains("userAccess"))
            {
                var ua = JsonSerializer.Deserialize<UserAccess>(context.Session.GetString("userAccess")!)!;
                context.User = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new Claim[]
                        {
                            new (ClaimTypes.Name, ua!.UserData.FirstName),
                            new (ClaimTypes.Surname, ua!.UserData.LastName),
                            new (ClaimTypes.Email, ua!.UserData.Email),
                        },
                        nameof(AuthSessionMiddleware)
                    )
                );
            }
            await _next(context);
        }
    }
}
