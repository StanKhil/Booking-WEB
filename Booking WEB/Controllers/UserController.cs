using Microsoft.AspNetCore.Mvc;

namespace Booking_WEB.Controllers
{
    public class UserController : Controller
    {
        public IActionResult SignUp()
        {
            if(Request.Method == "POST")
            {
                return RedirectToAction(nameof(SignIn));
            }
            return View();
        }
    }
}
