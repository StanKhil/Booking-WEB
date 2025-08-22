using Booking_WEB.Data;
using Booking_WEB.Data.DataAccessors;
using Booking_WEB.Data.Entities;
using Booking_WEB.Models.Realty;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace Booking_WEB.Controllers
{
    public class RealtyController(
        RealtyAccessor realtyAccessor) : Controller
    {
        private readonly RealtyAccessor _realtyAccessor = realtyAccessor;

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Create(CreateRealtyFormModel model)
        {
            try
            {
                String ext = model.Image.FileName[model.Image.FileName.LastIndexOf('.')..];
                String savedName = Guid.NewGuid() + ext;
                String path = @"C:\storage\" + savedName;
                using Stream stream = new StreamWriter(path).BaseStream;
                var copyTask = model.Image.CopyToAsync(stream);
                await copyTask;

                var cityId = await _realtyAccessor.GetCityIdByNameAsync(model.City);
                var countryId = await _realtyAccessor.GetCountryIdByNameAsync(model.Country);
                var groupId = await _realtyAccessor.GetGroupIdByNameAsync(model.Group);

                if (model == null)
                {
                    return Json(new
                    {
                        Status = 400,
                        Error = "Invalid JSON"
                    });
                }

                if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Slug))
                {
                    return Json(new
                    {
                        Status = 400,
                        Error = "Name and Slug are required"
                    });
                }

                var exists = await _realtyAccessor.SlugExistsAsync(model.Slug);

                if (exists)
                {
                    return Json(new
                    {
                        Status = 409,
                        Error = "Slug already exists"
                    });
                }

                var realty = new Realty
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Description = model.Description,
                    Slug = model.Slug,
                    ImageUrl = path,
                    Price = model.Price,
                    CityId = cityId,
                    CountryId = countryId,
                    GroupId = groupId,
                    DeletedAt = null
                };

                await _realtyAccessor.CreateAsync(realty);

                return Json(new
                {
                    Status = 200,
                    Message = "Realty created",
                    Data = new
                    {
                        realty.Id,
                        realty.Name,
                        realty.Slug
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
                var model = JsonSerializer.Deserialize<Realty>(body, options);

                if (model == null || model.Id == Guid.Empty)
                {
                    return Json(new { Status = 400, Error = "Invalid input" });
                }

                var realty = await _realtyAccessor.GetByIdAsync(model.Id, true);

                if (realty == null)
                {
                    return Json(new { Status = 404, Error = "Realty not found" });
                }

                var slugExists = await _realtyAccessor.SlugExistsAsync(model.Slug!, model.Id);

                if (slugExists)
                {
                    return Json(new { Status = 409, Error = "Slug already exists" });
                }

                if(!String.IsNullOrEmpty(model.Name)) realty.Name = model.Name;
                if (!String.IsNullOrEmpty(model.Description)) realty.Description = model.Description;
                if (!String.IsNullOrEmpty(model.Slug)) realty.Slug = model.Slug;
                if (!String.IsNullOrEmpty(model.ImageUrl)) realty.ImageUrl = model.ImageUrl;

                if(model.Price > 0) realty.Price = model.Price;

                realty.CityId = model.CityId;
                realty.CountryId = model.CountryId;
                realty.GroupId = model.GroupId;

                await _realtyAccessor.UpdateAsync(realty);

                return Json(new
                {
                    Status = 200,
                    Message = "Realty updated",
                    Data = new { realty.Id }
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

                var realty = await _realtyAccessor.GetByIdAsync(id, true);

                if (realty == null)
                {
                    return Json(new { Status = 404, Error = "Realty not found" });
                }

                await _realtyAccessor.SoftDeleteAsync(realty);

                return Json(new
                {
                    Status = 200,
                    Message = "Realty deleted",
                    Data = new { realty.Id }
                });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 500, Error = ex.Message });
            }
        }

    }
}
