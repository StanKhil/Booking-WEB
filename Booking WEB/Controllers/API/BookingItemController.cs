using Booking_WEB.Data;
using Booking_WEB.Data.DataAccessors;
using Booking_WEB.Data.Entities;
using Booking_WEB.Models.Booking;
using Booking_WEB.Models.Feedback;
using Booking_WEB.Models.Realty;
using Booking_WEB.Models.Rest;
using Microsoft.AspNetCore.Mvc;

namespace Booking_WEB.Controllers.API
{
    [ApiController]
    [Route("api/booking-item")]
    public class BookingItemController(
        BookingItemAccessor bookingItemAccessor,
        RealtyAccessor realtyAccessor,
        UserAccessAccessor userAccessAccessor,
        ILogger<BookingItemController> logger) : ControllerBase
    {
        private readonly BookingItemAccessor _bookingItemAccessor = bookingItemAccessor;
        private readonly RealtyAccessor _realtyAccessor = realtyAccessor;
        private readonly UserAccessAccessor _userAccessAccessor = userAccessAccessor;
        private readonly ILogger<BookingItemController> _logger = logger;

        [HttpPost]
        public async Task<ActionResult<RestResponse>> Create([FromBody] CreateBookingApiModel model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new RestResponse
                    {
                        Status = RestStatus.RestStatus400,
                        Meta = BuildMeta("Create"),
                        Data = "Invalid JSON"
                    });
                }

                if (model.StartDate == default || model.EndDate == default ||
                    model.UserAccessId == Guid.Empty || model.RealtyId == Guid.Empty)
                {
                    return BadRequest(new RestResponse
                    {
                        Status = RestStatus.RestStatus400,
                        Meta = BuildMeta("Create"),
                        Data = "StartDate, EndDate, UserAccessId and RealtyId are required"
                    });
                }

                if (model.StartDate >= model.EndDate)
                {
                    return BadRequest(new RestResponse
                    {
                        Status = RestStatus.RestStatus400,
                        Meta = BuildMeta("Create"),
                        Data = "StartDate must be before EndDate"
                    });
                }

                if (model.StartDate < DateTime.UtcNow.Date.AddDays(3))
                {
                    return BadRequest(new RestResponse
                    {
                        Status = RestStatus.RestStatus400,
                        Meta = BuildMeta("Create"),
                        Data = "StartDate must be at least 3 days from today."
                    });
                }


                var realty = await _realtyAccessor.GetRealtyBySlugAsync(model.RealtyId.ToString(), true);
                if (realty == null)
                {
                    return NotFound(new RestResponse
                    {
                        Status = new RestStatus { Code = 404, IsOk = false, Phrase = "Realty not found" },
                        Meta = BuildMeta("Create"),
                        Data = null
                    });
                }

                bool hasOverlap = realty.BookingItems.Any(b => b.DeletedAt == null &&
                    (model.StartDate >= b.StartDate && model.StartDate < b.EndDate ||
                     model.EndDate > b.StartDate && model.EndDate <= b.EndDate ||
                     model.StartDate <= b.StartDate && model.EndDate >= b.EndDate));

                if (hasOverlap)
                {
                    return Conflict(new RestResponse
                    {
                        Status = new RestStatus { Code = 409, IsOk = false, Phrase = "Booking overlap" },
                        Meta = BuildMeta("Create"),
                        Data = "This realty is already booked for the selected dates."
                    });
                }

                var userAccess = await _userAccessAccessor.GetByIdAsync(model.UserAccessId, true);
                if (userAccess == null)
                {
                    return NotFound(new RestResponse
                    {
                        Status = new RestStatus { Code = 404, IsOk = false, Phrase = "UserAccess not found" },
                        Meta = BuildMeta("Create"),
                        Data = null
                    });
                }

                var bookingItem = new BookingItem
                {
                    Id = Guid.NewGuid(),
                    RealtyId = realty.Id,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    CreatedAt = DateTime.Now,
                    DeletedAt = null,
                    UserAccessId = model.UserAccessId,
                    Realty = realty,
                    UserAccess = userAccess
                };

                await _bookingItemAccessor.CreateAsync(bookingItem);

                return Ok(new RestResponse
                {
                    Status = new RestStatus { Code = 200, IsOk = true, Phrase = "Created" },
                    Meta = BuildMeta("Create"),
                    Data = new
                    {
                        bookingItem.Id,
                        bookingItem.StartDate,
                        bookingItem.EndDate,
                        bookingItem.RealtyId,
                        bookingItem.UserAccessId
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"BookingItem/Create: {ex.Message}");
                return StatusCode(500, new RestResponse
                {
                    Status = RestStatus.RestStatus500,
                    Meta = BuildMeta("Create"),
                    Data = ex.Message
                });
            }
        }

        [HttpPatch]
        public async Task<ActionResult<RestResponse>> Update([FromBody] UpdateBookingApiModel model)
        {
            try
            {
                if (model == null || model.Id == Guid.Empty)
                {
                    return BadRequest(new RestResponse
                    {
                        Status = RestStatus.RestStatus400,
                        Meta = BuildMeta("Update"),
                        Data = "Invalid input"
                    });
                }

                var bookingItem = await _bookingItemAccessor.GetByIdAsync(model.Id, true);
                if (bookingItem == null)
                {
                    return NotFound(new RestResponse
                    {
                        Status = new RestStatus { Code = 404, IsOk = false, Phrase = "BookingItem not found" },
                        Meta = BuildMeta("Update"),
                        Data = null
                    });
                }

                if (model.StartDate >= model.EndDate)
                {
                    return BadRequest(new RestResponse
                    {
                        Status = RestStatus.RestStatus400,
                        Meta = BuildMeta("Update"),
                        Data = "StartDate must be before EndDate"
                    });
                }

                if (model.StartDate < DateTime.UtcNow.Date.AddDays(3))
                {
                    return BadRequest(new RestResponse
                    {
                        Status = RestStatus.RestStatus400,
                        Meta = BuildMeta("Create"),
                        Data = "StartDate must be at least 3 days from today."
                    });
                }

                bool hasOverlap = await _bookingItemAccessor.HasOverlapAsync(
                    model.RealtyId, model.StartDate, model.EndDate, model.Id);

                if (hasOverlap)
                {
                    return Conflict(new RestResponse
                    {
                        Status = new RestStatus { Code = 409, IsOk = false, Phrase = "Booking overlap" },
                        Meta = BuildMeta("Update"),
                        Data = "This realty is already booked for the selected dates."
                    });
                }

                bookingItem.StartDate = model.StartDate;
                bookingItem.EndDate = model.EndDate;
                bookingItem.RealtyId = model.RealtyId;
                bookingItem.UserAccessId = model.UserAccessId;

                await _bookingItemAccessor.UpdateAsync(bookingItem);

                bool isAlreadyFeedbacked = bookingItem.UserAccess.Feedbacks.Any(f => f.RealtyId == bookingItem.RealtyId);
                return Ok(new RestResponse
                {
                    Status = new RestStatus { Code = 200, IsOk = true, Phrase = "Updated" },
                    Meta = BuildMeta("Update"),
                    Data = new {
                        bookingItem.Id,
                        bookingItem.StartDate,
                        bookingItem.EndDate,
                        bookingItem.RealtyId,
                        bookingItem.UserAccess,
                        bookingItem.Realty,
                        isAlreadyFeedbacked
                    }
                });
            }
            catch (Exception ex)
            {
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
                var bookingItem = await _bookingItemAccessor.GetByIdAsync(id, true);
                if (bookingItem == null)
                {
                    return NotFound(new RestResponse
                    {
                        Status = new RestStatus { Code = 404, IsOk = false, Phrase = "BookingItem not found" },
                        Meta = BuildMeta("Delete"),
                        Data = null
                    });
                }

                await _bookingItemAccessor.SoftDeleteAsync(bookingItem);

                return Ok(new RestResponse
                {
                    Status = new RestStatus { Code = 200, IsOk = true, Phrase = "Deleted" },
                    Meta = BuildMeta("Delete"),
                    Data = new { bookingItem.Id }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RestResponse
                {
                    Status = RestStatus.RestStatus500,
                    Meta = BuildMeta("Delete"),
                    Data = ex.Message
                });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<RestResponse>> GetById(Guid id)
        {
            try
            {
                var bookingItem = await _bookingItemAccessor.GetByIdAsync(id, false);
                if (bookingItem == null)
                {
                    return NotFound(new RestResponse
                    {
                        Status = new RestStatus { Code = 404, IsOk = false, Phrase = "BookingItem not found" },
                        Meta = BuildMeta("GetById"),
                        Data = null
                    });
                }
                bool isAlreadyFeedbacked = bookingItem.UserAccess.Feedbacks.Any(f => f.RealtyId == bookingItem.RealtyId);
                return Ok(new RestResponse
                {
                    Status = new RestStatus { Code = 200, IsOk = true, Phrase = "OK" },
                    Meta = BuildMeta("GetById"),
                    Data = new
                    {
                        bookingItem.Id,
                        bookingItem.StartDate,
                        bookingItem.EndDate,
                        bookingItem.RealtyId,
                        bookingItem.UserAccess,
                        bookingItem.Realty,
                        isAlreadyFeedbacked
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RestResponse
                {
                    Status = RestStatus.RestStatus500,
                    Meta = BuildMeta("GetById"),
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
