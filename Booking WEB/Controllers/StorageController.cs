using Microsoft.AspNetCore.Mvc;

namespace Booking_WEB.Controllers
{
    public class StorageController : Controller
    {
        [HttpGet]
        public IActionResult Index(string id)
        {
            string path = @"C:\storage\" + id;
            string path2 = @"D:\C#\ASP\storage\" + id; 
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
