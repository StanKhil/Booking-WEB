using System.Diagnostics;
using Booking_WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Booking_WEB.Models.Realty;

namespace Booking_WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

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
