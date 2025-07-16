using Booking_WEB.Data;
using Booking_WEB.Services.Identity;
using Booking_WEB.Services.Kdf;
using Booking_WEB.Services.Random;
using Booking_WEB.Services.Time;
using Microsoft.EntityFrameworkCore;

namespace Booking_WEB
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<IRandomService, DefaultRandomService>();
            builder.Services.AddSingleton<ITimeService, MilliSecTimeService>();
            builder.Services.AddSingleton<IIdentityService, DefaultIdentityService>();
            builder.Services.AddSingleton<IKdfService, PbKdfService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<DataContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDB"))
            );

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromSeconds(100);
                options.Cookie.HttpOnly = true; options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            using (var scope = app.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<DataContext>(); await db.Database.MigrateAsync(); }

            app.Run();
        }
    }
}
