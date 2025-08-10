using Microsoft.AspNetCore.Mvc;

namespace Booking_WEB.Controllers
{
    public class ResourcesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //Example of request
        //GET//Resources/GetHtmlPage?fileName=${fileName}

        [HttpGet]
        public async Task<IActionResult> GetHtmlPage([FromQuery] string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return BadRequest("File name is required");
            }

            fileName = Path.GetFileName(fileName);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "resources", "pages", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            var htmlContent = await System.IO.File.ReadAllTextAsync(filePath);
            return Content(htmlContent, "text/html");
        }
    }
}
