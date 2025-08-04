using Booking_WEB.Data;
using Booking_WEB.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Booking_WEB.Controllers
{
    public class FeedbackController(DataContext context) : Controller
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

                var model = JsonSerializer.Deserialize<Feedback>(body, options);

                if (model == null || string.IsNullOrWhiteSpace(model.Text) || model.RealtyId == Guid.Empty || model.UserAccessId == Guid.Empty)
                {
                    return Json(new { Status = 400, Error = "Invalid feedback data" });
                }

                if (model.Rate < 1 || model.Rate > 5)
                {
                    return Json(new { Status = 400, Error = "Rate must be between 1 and 5" });
                }

                var realty = await _context.Realties
                    .Include(r => r.Feedbacks)
                    .FirstOrDefaultAsync(r => r.Id == model.RealtyId && r.DeletedAt == null);

                if (realty == null)
                {
                    return Json(new { Status = 404, Error = "Realty not found" });
                }

                var userAccess = await _context.UserAccesses
                    .Include(u => u.Feedbacks)
                    .FirstOrDefaultAsync(u => u.Id == model.UserAccessId);

                if (userAccess == null)
                {
                    return Json(new { Status = 404, Error = "User not found" });
                }

                var feedback = new Feedback
                {
                    Id = Guid.NewGuid(),
                    RealtyId = model.RealtyId,
                    UserAccessId = model.UserAccessId,
                    Text = model.Text,
                    Rate = model.Rate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    DeletedAt = null,
                    Realty = realty,
                    UserAccess = userAccess
                };

                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    Status = 200,
                    Message = "Feedback created",
                    Data = new { feedback.Id, feedback.Text, feedback.Rate }
                });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 500, Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> Update()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var model = JsonSerializer.Deserialize<Feedback>(body, options);

                if (model == null || model.Id == Guid.Empty || string.IsNullOrWhiteSpace(model.Text))
                {
                    return Json(new { Status = 400, Error = "Invalid feedback data" });
                }

                var feedback = await _context.Feedbacks
                    .FirstOrDefaultAsync(f => f.Id == model.Id && f.DeletedAt == null);

                if (feedback == null)
                {
                    return Json(new { Status = 404, Error = "Feedback not found" });
                }

                if (model.Rate < 1 || model.Rate > 5)
                {
                    return Json(new { Status = 400, Error = "Rate must be between 1 and 5" });
                }

                feedback.Text = model.Text;
                feedback.Rate = model.Rate;
                feedback.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Json(new { Status = 200, Message = "Feedback updated", Data = new { feedback.Id } });
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

                if (!json.RootElement.TryGetProperty("id", out var idElement) ||
                    !Guid.TryParse(idElement.ToString(), out var feedbackId))
                {
                    return Json(new { Status = 400, Error = "Invalid ID" });
                }

                var feedback = await _context.Feedbacks
                    .FirstOrDefaultAsync(f => f.Id == feedbackId && f.DeletedAt == null);

                if (feedback == null)
                {
                    return Json(new { Status = 404, Error = "Feedback not found" });
                }

                feedback.DeletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Json(new { Status = 200, Message = "Feedback deleted", Data = new { feedback.Id } });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 500, Error = ex.Message });
            }
        }
    }
}
