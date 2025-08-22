using Microsoft.AspNetCore.Mvc;

namespace Booking_WEB.Controllers
{
    public class StorageController : Controller
    {
        [HttpGet]
        public IActionResult Index(String id)
        {
            String path = @"C:\storage\" + id;
            String path2 = @"DimaPath\" + id; //поставь свой путь
            if (System.IO.File.Exists(path))
            {
                return File(System.IO.File.ReadAllBytes(path), "image/jpeg");
            }
            if (System.IO.File.Exists(path2))
            {
                return File(System.IO.File.ReadAllBytes(path2), "image/jpeg");
            }
            return NotFound();
        }
    }
}
