using Booking_WEB.Data;
using Booking_WEB.Services.Identity;
using Booking_WEB.Services.Kdf;
using Booking_WEB.Services.Random;
using Booking_WEB.Services.Time;
using Microsoft.EntityFrameworkCore;
using Booking_WEB.Middleware.Auth;
using Booking_WEB.Services.Jwt;
using Booking_WEB.Data.DataAccessors;
using Microsoft.AspNetCore.Identity;
using Booking_WEB.Services.Storage;

namespace Booking_WEB
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("config.json", optional: false, reloadOnChange: true);
            builder.Services.Configure<StorageOptions>(
                builder.Configuration);

            builder.Services.AddControllersWithViews();

            builder.Services.AddSingleton<IRandomService, DefaultRandomService>();
            builder.Services.AddSingleton<ITimeService, SecTimeService>();
            builder.Services.AddSingleton<IIdentityService, DefaultIdentityService>();
            builder.Services.AddSingleton<IKdfService, PbKdfService>();
            builder.Services.AddSingleton<IJwtService, JwtService>();
            builder.Services.AddSingleton<IStorageService, DiskStorageService>();

            builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDB")));

            builder.Services.AddScoped<UserAccessAccessor>();
            builder.Services.AddScoped<AccessTokenAccessor>();
            builder.Services.AddScoped<RealtyAccessor>();
            builder.Services.AddScoped<UserDataAccessor>();
            builder.Services.AddScoped<BookingItemAccessor>();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options => 
            {
                options.IdleTimeout = TimeSpan.FromSeconds(100);
                options.Cookie.HttpOnly = true; 
                options.Cookie.IsEssential = true;
            });


            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.MapStaticAssets();

            app.UseSession();

            app.UseAuthSession();
            app.UseAuthJwt();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            using (var scope = app.Services.CreateScope()) 
            { 
                var db = scope.ServiceProvider.GetRequiredService<DataContext>(); 
                await db.Database.MigrateAsync(); 
            }

            app.Run();
        }
    }
}
