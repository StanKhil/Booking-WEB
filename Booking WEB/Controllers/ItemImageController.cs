using Booking_WEB.Data;
using Booking_WEB.Data.DataAccessors;
using Booking_WEB.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Booking_WEB.Controllers
{
    public class ItemImageController(ItemImageAccessor itemImageAccessor) : Controller
    {
        private readonly ItemImageAccessor _itemImageAccessor = itemImageAccessor ?? throw new Exception("ItemImageAccessor not found");

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

                var existingUrls = await _itemImageAccessor.GetUrlsByItemIdAsync(data.RealtyId);

                var newUrls = data.Urls
                    .Where(url => !existingUrls.Contains(url))
                    .Distinct()
                    .ToList();

                if (newUrls.Count > 0)
                {
                    await _itemImageAccessor.AddRangeAsync(data.RealtyId, newUrls);
                }

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

                var deletedCount = await _itemImageAccessor.DeleteByItemIdAsync(itemId);

                if (deletedCount == 0)
                {
                    return Json(new { Status = 404, Error = "No images found for given itemId" });
                }

                return Json(new
                {
                    Status = 200,
                    Message = "Images deleted",
                    DeletedCount = deletedCount
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
