using Booking_WEB.Data;
using Booking_WEB.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Booking_WEB.Controllers
{
    public class ItemImageController(DataContext context) : Controller
    {
        private readonly DataContext _context = context ?? throw new Exception("context not found");

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Create()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = JsonSerializer.Deserialize<CreateItemImageRequest>(body, options);

                if (data == null || data.RealtyId == Guid.Empty || data.Urls == null || !data.Urls.Any())
                {
                    return Json(new
                    {
                        Status = 400,
                        Error = "RealtyId and at least one Url are required."
                    });
                }

                var existingUrls = await _context.ItemImages
                    .Where(i => i.ItemId == data.RealtyId)
                    .Select(i => i.ImageUrl)
                    .ToListAsync();

                var newUrls = data.Urls
                    .Where(url => !existingUrls.Contains(url))
                    .Distinct()
                    .ToList();

                foreach (var url in newUrls)
                {
                    var image = new ItemImage
                    {
                        ItemId = data.RealtyId,
                        ImageUrl = url,
                        Order = 0
                    };
                    _context.ItemImages.Add(image);
                }

                await _context.SaveChangesAsync();

                return Json(new
                {
                    Status = 200,
                    Message = "Images created",
                    CreatedCount = newUrls.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 500, Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> Delete()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                var json = JsonDocument.Parse(body);
                var itemIdProp = json.RootElement.GetProperty("itemId");

                if (!Guid.TryParse(itemIdProp.ToString(), out var itemId))
                {
                    return Json(new { Status = 400, Error = "Invalid itemId" });
                }

                var images = await _context.ItemImages
                    .Where(i => i.ItemId == itemId)
                    .ToListAsync();

                if (!images.Any())
                {
                    return Json(new { Status = 404, Error = "No images found for given itemId" });
                }

                _context.ItemImages.RemoveRange(images);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    Status = 200,
                    Message = "Images deleted",
                    DeletedCount = images.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 500, Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> LoadImage()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                var json = JsonDocument.Parse(body);
                var root = json.RootElement;

                if (!root.TryGetProperty("slug", out var slugProp) ||
                    !root.TryGetProperty("localFilePath", out var pathProp))
                {
                    return Json(new { Status = 400, Error = "Missing slug or localFilePath" });
                }

                string slug = slugProp.GetString() ?? "";
                string localFilePath = pathProp.GetString() ?? "";

                if (string.IsNullOrWhiteSpace(slug) || string.IsNullOrWhiteSpace(localFilePath))
                {
                    return Json(new { Status = 400, Error = "Invalid slug or localFilePath" });
                }

                string result = await LoadImageAsync(slug, localFilePath);

                return result == "Created"
                    ? Json(new { Status = 200, Message = "Image copied successfully" })
                    : Json(new { Status = 500, Error = result });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 500, Error = ex.Message });
            }
        }

        private async Task<string> LoadImageAsync(string slug, string localFilePath)
        {
            try
            {
                string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
                string folderPath = Path.Combine(projectRoot, "Images", slug);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                localFilePath = localFilePath.Trim().Replace("\u202A", "").Replace("\u200E", "");

                if (!System.IO.File.Exists(localFilePath))
                {
                    return "Local image file not found.";
                }

                string fileName = Path.GetFileName(localFilePath);
                string destPath = Path.Combine(folderPath, fileName);

                System.IO.File.Copy(localFilePath, destPath, overwrite: true);

                return "Created";
            }
            catch (Exception ex)
            {
                return $"Error copying image: {ex.Message}";
            }
        }

        private class CreateItemImageRequest
        {
            public Guid RealtyId { get; set; }
            public List<string> Urls { get; set; } = [];
        }

    }
}
