using Booking_WEB.Data;
using Booking_WEB.Data.DataAccessors;
using Booking_WEB.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Booking_WEB.Controllers
{
    public class FeedbackController(
        UserAccessAccessor userAccessAccessor,
        RealtyAccessor realtyAccessor,
        FeedbackAccessor feedbackAccessor) : Controller
    {

        private readonly UserAccessAccessor _userAccessAccessor = userAccessAccessor ?? throw new ArgumentNullException(nameof(userAccessAccessor));
        private readonly RealtyAccessor _realtyAccessor = realtyAccessor ?? throw new ArgumentNullException(nameof(realtyAccessor));
        private readonly FeedbackAccessor _feedbackAccessor = feedbackAccessor ?? throw new ArgumentNullException(nameof(feedbackAccessor));

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

                var realty = await _realtyAccessor.GetRealtyBySlugAsync(model.RealtyId.ToString(), isEditable: false);

                if (realty == null)
                {
                    return Json(new { Status = 404, Error = "Realty not found" });
                }

                var userAccess = await _userAccessAccessor.GetByIdAsync(model.UserAccessId, isEditable: false);

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

                await _feedbackAccessor.CreateAsync(feedback);

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

                var feedback = await _feedbackAccessor.GetByIdAsync(model.Id, isEditable: true);

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

                await _feedbackAccessor.UpdateAsync(feedback);

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

                var feedback = await _feedbackAccessor.GetByIdAsync(feedbackId, isEditable: true);

                if (feedback == null)
                {
                    return Json(new { Status = 404, Error = "Feedback not found" });
                }

                await _feedbackAccessor.SoftDeleteAsync(feedback);

                return Json(new { Status = 200, Message = "Feedback deleted", Data = new { feedback.Id } });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 500, Error = ex.Message });
            }
        }
    }
}
