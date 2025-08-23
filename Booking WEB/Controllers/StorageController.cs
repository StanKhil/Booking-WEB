using Booking_WEB.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Booking_WEB.Controllers
{
    public class StorageController(IStorageService storageService) : Controller
    {
        private readonly IStorageService _storageService = storageService;
        [HttpGet]
        public IActionResult Item(String id)
        {
            try
            {
                return File(
                    _storageService.GetItemBytes(id),
                    _storageService.TryGetMimeType(id)
                );
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
