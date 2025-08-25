using System.Diagnostics;
using Booking_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Booking_WEB.Models.Realty;
using System.Security.Claims;
using Booking_WEB.Data.DataAccessors;

namespace Booking_WEB.Controllers
{
    public class HomeController(ILogger<HomeController> logger, BookingItemAccessor bookingItemAccessor) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly BookingItemAccessor _bookingItemAccessor = bookingItemAccessor;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }
        public IActionResult BookingsAndTrips()
        {
            string? login = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;
            if(login != null) ViewData["Bookings"] = _bookingItemAccessor.GetListByUserLoginAsync(login).Result;
            return View();
        }
        public IActionResult Administrator()
        {
            return View();
        }
        public IActionResult Item()
        {
            RealtyModel model = new(); // TO DO: fill in the data
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
