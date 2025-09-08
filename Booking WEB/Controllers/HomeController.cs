using System.Diagnostics;
using Booking_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Booking_WEB.Models.Realty;
using System.Security.Claims;
using Booking_WEB.Data.DataAccessors;
using Booking_WEB.Data.Entities;

namespace Booking_WEB.Controllers
{
    public class HomeController(ILogger<HomeController> logger, BookingItemAccessor bookingItemAccessor, RealtyAccessor realtyAccessor) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly BookingItemAccessor _bookingItemAccessor = bookingItemAccessor;
        private readonly RealtyAccessor _realtyAccessor = realtyAccessor;

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
            BookingsAndTripsModel model = new();
            string? login = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid)?.Value;
            if (login != null)
            {
                model.BookingItems = _bookingItemAccessor.GetBookingItemsByUserLoginAsync(login).Result;
            }
            return View(model);
        }
        public IActionResult Administrator()
        {
            return View();
        }
        public async Task<IActionResult> ItemAsync([FromRoute]string id)
        {
            Realty? realty = null;
            try
            {
                realty = await _realtyAccessor.GetRealtyBySlugAsync(id);
            }
            catch(Exception e)
            {
                _logger.LogError("Exception: " + e.Message);
            }
            RealtyModel? model = null;
            if (realty != null)
            {
                model = new();
                model.Name = realty.Name;
                model.Price = realty.Price;
                model.Description = realty.Description;
                model.City = realty.City;
                model.Feedbacks = realty.Feedbacks;
                model.Images = realty.Images;
                model.AccRates = realty.AccRates;
            }
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
