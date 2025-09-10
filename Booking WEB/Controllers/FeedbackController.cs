using Booking_WEB.Data;
using Booking_WEB.Data.DataAccessors;
using Booking_WEB.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Booking_WEB.Models.Rest;

namespace Booking_WEB.Controllers
{
    [ApiController]
    [Route("api/feedback")]
    public class FeedbackController(
        UserAccessAccessor userAccessAccessor,
        RealtyAccessor realtyAccessor,
        FeedbackAccessor feedbackAccessor) : Controller
    {

        private readonly UserAccessAccessor _userAccessAccessor = userAccessAccessor ?? throw new ArgumentNullException(nameof(userAccessAccessor));
        private readonly RealtyAccessor _realtyAccessor = realtyAccessor ?? throw new ArgumentNullException(nameof(realtyAccessor));
        private readonly FeedbackAccessor _feedbackAccessor = feedbackAccessor ?? throw new ArgumentNullException(nameof(feedbackAccessor));

        private RestMeta BuildMeta(string action, string method = "GET")
        {
            return new RestMeta
            {
                ServerTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                ResourceName = "Feedback",
                ResourceUrl = HttpContext.Request.Path,
                Cache = 0,
                Manipulations = new[] { action },
                Links = new Dictionary<string, string>
                {
                    { "self", HttpContext.Request.Path }
                },
                Method = method,
                DataType = "application/json"
            };
        }

        [HttpPost]
        public async Task<ActionResult<RestResponse>> Create([FromBody] Feedback model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Text) ||
                model.RealtyId == Guid.Empty || model.UserAccessId == Guid.Empty)
            {
                return BadRequest(new RestResponse
                {
                    Status = RestStatus.RestStatus400,
                    Meta = BuildMeta("Create", "POST"),
                    Data = "Invalid feedback data"
                });
            }

            if (model.Rate < 1 || model.Rate > 5)
            {
                return BadRequest(new RestResponse
                {
                    Status = RestStatus.RestStatus400,
                    Meta = BuildMeta("Create", "POST"),
                    Data = "Rate must be between 1 and 5"
                });
            }

            var realty = await _realtyAccessor.GetRealtyByIdAsync(model.RealtyId);
            if (realty == null)
                return NotFound(new RestResponse
                {
                    Status = new RestStatus { Code = 404, IsOk = false, Phrase = "Realty not found" },
                    Meta = BuildMeta("Create", "POST"),
                    Data = null
                });

            var userAccess = await _userAccessAccessor.GetByIdAsync(model.UserAccessId);
            if (userAccess == null)
                return NotFound(new RestResponse
                {
                    Status = new RestStatus { Code = 404, IsOk = false, Phrase = "User not found" },
                    Meta = BuildMeta("Create", "POST"),
                    Data = null
                });

            var feedback = new Feedback
            {
                Id = Guid.NewGuid(),
                RealtyId = model.RealtyId,
                UserAccessId = model.UserAccessId,
                Text = model.Text,
                Rate = model.Rate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _feedbackAccessor.CreateAsync(feedback);

            return StatusCode(201, new RestResponse
            {
                Status = RestStatus.RestStatus201,
                Meta = BuildMeta("Create", "POST"),
                Data = new { feedback.Id, feedback.Text, feedback.Rate }
            });
        }

        [HttpPatch("{id:guid}")]
        public async Task<ActionResult<RestResponse>> Update(Guid id, [FromBody] Feedback model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Text))
                return BadRequest(new RestResponse
                {
                    Status = RestStatus.RestStatus400,
                    Meta = BuildMeta("Update", "PATCH"),
                    Data = "Invalid feedback data"
                });

            var feedback = await _feedbackAccessor.GetByIdAsync(id, isEditable: true);
            if (feedback == null)
                return NotFound(new RestResponse
                {
                    Status = new RestStatus { Code = 404, IsOk = false, Phrase = "Feedback not found" },
                    Meta = BuildMeta("Update", "PATCH"),
                    Data = null
                });

            if (model.Rate < 1 || model.Rate > 5)
                return BadRequest(new RestResponse
                {
                    Status = RestStatus.RestStatus400,
                    Meta = BuildMeta("Update", "PATCH"),
                    Data = "Rate must be between 1 and 5"
                });

            feedback.Text = model.Text;
            feedback.Rate = model.Rate;
            feedback.UpdatedAt = DateTime.UtcNow;

            await _feedbackAccessor.UpdateAsync(feedback);

            return Ok(new RestResponse
            {
                Status = new RestStatus { Code = 200, IsOk = true, Phrase = "Updated" },
                Meta = BuildMeta("Update", "PATCH"),
                Data = new { feedback.Id }
            });
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<RestResponse>> Delete(Guid id)
        {
            var feedback = await _feedbackAccessor.GetByIdAsync(id, isEditable: true);
            if (feedback == null)
                return NotFound(new RestResponse
                {
                    Status = new RestStatus { Code = 404, IsOk = false, Phrase = "Feedback not found" },
                    Meta = BuildMeta("Delete", "DELETE"),
                    Data = null
                });

            await _feedbackAccessor.SoftDeleteAsync(feedback);

            return Ok(new RestResponse
            {
                Status = new RestStatus { Code = 200, IsOk = true, Phrase = "Deleted" },
                Meta = BuildMeta("Delete", "DELETE"),
                Data = new { feedback.Id }
            });
        }

    }
}
