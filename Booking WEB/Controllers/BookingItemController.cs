using Booking_WEB.Data;
using Booking_WEB.Data.DataAccessors;
using Booking_WEB.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace Booking_WEB.Controllers
{
    public class BookingItemController(
        BookingItemAccessor bookingItemAccessor,
        RealtyAccessor realtyAccessor,
        UserAccessAccessor userAccessAccessor) : Controller
    {
        private readonly BookingItemAccessor bookingItemAccessor = bookingItemAccessor;
        private readonly RealtyAccessor realtyAccessor = realtyAccessor;
        private readonly UserAccessAccessor userAccessAccessor = userAccessAccessor;

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
                var model = JsonSerializer.Deserialize<BookingItem>(body, options);

                if (model == null)
                {
                    return Json(new
                    {
                        Status = 400,
                        Error = "Invalid JSON"
                    });
                }

                if(model.StartDate == default || model.EndDate == default ||model.UserAccessId == Guid.Empty || model.RealtyId == Guid.Empty)
                {
                    return Json(new
                    {
                        Status = 400,
                        Error = "StartDate, EndDate, UserAccessId and RealtyId are required"
                    });
                }

                if (model.StartDate >= model.EndDate)
                {
                    return Json(new
                    {
                        Status = 400,
                        Error = "StartDate must be before EndDate"
                    });
                }

                var realty = await realtyAccessor.GetByIdAsync(model.RealtyId, true);

                if (realty == null)
                {
                    return Json(new
                    {
                        Status = 404,
                        Error = "Realty not found"
                    });
                }

                bool hasOverlap = realty.BookingItems
                    .Any(b => b.DeletedAt == null &&
                (
                 (model.StartDate >= b.StartDate && model.StartDate < b.EndDate) ||
                 (model.EndDate > b.StartDate && model.EndDate <= b.EndDate) ||
                 (model.StartDate <= b.StartDate && model.EndDate >= b.EndDate))
                );

                if (hasOverlap)
                {
                    return  Json(new
                    {
                        Status = 409,
                        Error = "This realty is already booked for the selected dates."
                    });
                }

                BookingItem bookingItem = new BookingItem()
                {
                    Id = Guid.NewGuid(),
                    RealtyId = realty.Id,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    CreatedAt = DateTime.Now,
                    DeletedAt = null,
                    UserAccessId = model.UserAccessId
                };

                var userAccess = await userAccessAccessor.GetByIdAsync(model.UserAccessId, true);

                if (userAccess == null)
                    throw new InvalidOperationException("UserAccess not found.");

                bookingItem.UserAccess = userAccess;
                bookingItem.Realty = realty;

                await bookingItemAccessor.CreateAsync(bookingItem);

                return Json(new
                {
                    Status = 200,
                    Message = "Booking created",
                    Data = new
                    {
                        bookingItem.Id,
                        bookingItem.StartDate,
                        bookingItem.EndDate,
                        RealtyId = bookingItem.RealtyId,
                        UserAccessId = bookingItem.UserAccessId
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = 500,
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> Update()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var model = JsonSerializer.Deserialize<BookingItem>(body, options);

                if (model == null || model.Id == Guid.Empty)
                {
                    return Json(new { Status = 400, Error = "Invalid input" });
                }

                //var bookingItem = await _context.BookingItems
                //    .FirstOrDefaultAsync(b => b.Id == model.Id && b.DeletedAt == null);
                var bookingItem = await bookingItemAccessor.GetByIdAsync(model.Id, true);

                if (bookingItem == null)
                {
                    return Json(new { Status = 404, Error = "BookingItem not found" });
                }

                if (model.StartDate >= model.EndDate)
                {
                    return Json(new { Status = 400, Error = "StartDate must be before EndDate" });
                }

                bool hasOverlap = await bookingItemAccessor.HasOverlapAsync(
                    model.RealtyId, model.StartDate, model.EndDate, model.Id);

                if (hasOverlap)
                {
                    return Json(new
                    {
                        Status = 409,
                        Error = "This realty is already booked for the selected dates."
                    });
                }

                bookingItem.StartDate = model.StartDate;
                bookingItem.EndDate = model.EndDate;
                bookingItem.RealtyId = model.RealtyId;
                bookingItem.UserAccessId = model.UserAccessId;

                await bookingItemAccessor.UpdateAsync(bookingItem);

                return Json(new
                {
                    Status = 200,
                    Message = "BookingItem updated",
                    Data = new { bookingItem.Id }
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
                var idProp = json.RootElement.GetProperty("id");

                if (!Guid.TryParse(idProp.ToString(), out var id))
                {
                    return Json(new { Status = 400, Error = "Invalid ID" });
                }

                var bookingItem = await bookingItemAccessor.GetByIdAsync(id, true);

                if (bookingItem == null)
                {
                    return Json(new { Status = 404, Error = "BookingItem not found" });
                }

                await bookingItemAccessor.SoftDeleteAsync(bookingItem);

                return Json(new
                {
                    Status = 200,
                    Message = "BookingItem deleted",
                    Data = new { bookingItem.Id }
                });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 500, Error = ex.Message });
            }
        }

    }
}
