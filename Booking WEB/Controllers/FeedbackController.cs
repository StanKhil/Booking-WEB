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
    [Route("api/booking-item")]
    public class FeedbackController(
        UserAccessAccessor userAccessAccessor,
        RealtyAccessor realtyAccessor,
        FeedbackAccessor feedbackAccessor,
        ILogger<FeedbackController> logger) : ControllerBase
    {

        private readonly UserAccessAccessor _userAccessAccessor = userAccessAccessor ?? throw new ArgumentNullException(nameof(userAccessAccessor));
        private readonly RealtyAccessor _realtyAccessor = realtyAccessor ?? throw new ArgumentNullException(nameof(realtyAccessor));
        private readonly FeedbackAccessor _feedbackAccessor = feedbackAccessor ?? throw new ArgumentNullException(nameof(feedbackAccessor));
        private readonly ILogger<FeedbackController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpPost]
            public async Task<ActionResult<RestResponse>> Create([FromBody] Feedback model)
            {
                try
                {
                    if (model == null || string.IsNullOrWhiteSpace(model.Text) ||
                        model.RealtyId == Guid.Empty || model.UserAccessId == Guid.Empty)
                    {
                        return BadRequest(new RestResponse
                        {
                            Status = RestStatus.RestStatus400,
                            Meta = BuildMeta("Create"),
                            Data = "Invalid feedback data"
                        });
                    }

                    if (model.Rate < 1 || model.Rate > 5)
                    {
                        return BadRequest(new RestResponse
                        {
                            Status = RestStatus.RestStatus400,
                            Meta = BuildMeta("Create"),
                            Data = "Rate must be between 1 and 5"
                        });
                    }

                    var realty = await _realtyAccessor.GetByIdAsync(model.RealtyId, false);
                    if (realty == null)
                    {
                        return NotFound(new RestResponse
                        {
                            Status = new RestStatus { Code = 404, IsOk = false, Phrase = "Realty not found" },
                            Meta = BuildMeta("Create"),
                            Data = null
                        });
                    }

                    var userAccess = await _userAccessAccessor.GetByIdAsync(model.UserAccessId, false);
                    if (userAccess == null)
                    {
                        return NotFound(new RestResponse
                        {
                            Status = new RestStatus { Code = 404, IsOk = false, Phrase = "User not found" },
                            Meta = BuildMeta("Create"),
                            Data = null
                        });
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

                    return Ok(new RestResponse
                    {
                        Status = RestStatus.RestStatus201,
                        Meta = BuildMeta("Create"),
                        Data = new { feedback.Id, feedback.Text, feedback.Rate }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Feedback/Create: {ex.Message}");
                    return StatusCode(500, new RestResponse
                    {
                        Status = RestStatus.RestStatus500,
                        Meta = BuildMeta("Create"),
                        Data = ex.Message
                    });
                }
            }

            [HttpPatch]
            public async Task<ActionResult<RestResponse>> Update([FromBody] Feedback model)
            {
                try
                {
                    if (model == null || model.Id == Guid.Empty || string.IsNullOrWhiteSpace(model.Text))
                    {
                        return BadRequest(new RestResponse
                        {
                            Status = RestStatus.RestStatus400,
                            Meta = BuildMeta("Update"),
                            Data = "Invalid feedback data"
                        });
                    }

                    var feedback = await _feedbackAccessor.GetByIdAsync(model.Id, true);
                    if (feedback == null)
                    {
                        return NotFound(new RestResponse
                        {
                            Status = new RestStatus { Code = 404, IsOk = false, Phrase = "Feedback not found" },
                            Meta = BuildMeta("Update"),
                            Data = null
                        });
                    }

                    if (model.Rate < 1 || model.Rate > 5)
                    {
                        return BadRequest(new RestResponse
                        {
                            Status = RestStatus.RestStatus400,
                            Meta = BuildMeta("Update"),
                            Data = "Rate must be between 1 and 5"
                        });
                    }

                    feedback.Text = model.Text;
                    feedback.Rate = model.Rate;
                    feedback.UpdatedAt = DateTime.UtcNow;

                    await _feedbackAccessor.UpdateAsync(feedback);

                    return Ok(new RestResponse
                    {
                        Status = new RestStatus { Code = 200, IsOk = true, Phrase = "Updated" },
                        Meta = BuildMeta("Update"),
                        Data = new { feedback.Id }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Feedback/Update: {ex.Message}");
                    return StatusCode(500, new RestResponse
                    {
                        Status = RestStatus.RestStatus500,
                        Meta = BuildMeta("Update"),
                        Data = ex.Message
                    });
                }
            }

            [HttpDelete("{id:guid}")]
            public async Task<ActionResult<RestResponse>> Delete(Guid id)
            {
                try
                {
                    var feedback = await _feedbackAccessor.GetByIdAsync(id, true);
                    if (feedback == null)
                    {
                        return NotFound(new RestResponse
                        {
                            Status = new RestStatus { Code = 404, IsOk = false, Phrase = "Feedback not found" },
                            Meta = BuildMeta("Delete"),
                            Data = null
                        });
                    }

                    await _feedbackAccessor.SoftDeleteAsync(feedback);

                    return Ok(new RestResponse
                    {
                        Status = new RestStatus { Code = 200, IsOk = true, Phrase = "Deleted" },
                        Meta = BuildMeta("Delete"),
                        Data = new { feedback.Id }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Feedback/Delete: {ex.Message}");
                    return StatusCode(500, new RestResponse
                    {
                        Status = RestStatus.RestStatus500,
                        Meta = BuildMeta("Delete"),
                        Data = ex.Message
                    });
                }
            }

            private RestMeta BuildMeta(string resourceName)
            {
                return new RestMeta
                {
                    ResourceName = resourceName,
                    ResourceUrl = HttpContext.Request.Path,
                    DataType = "application/json",
                    Method = HttpContext.Request.Method
                };
            }
        }
}
