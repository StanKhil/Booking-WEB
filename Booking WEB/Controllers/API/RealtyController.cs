using Booking_WEB.Data;
using Booking_WEB.Data.DataAccessors;
using Booking_WEB.Data.Entities;
using Booking_WEB.Models.Realty;
using Booking_WEB.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using Booking_WEB.Filters;

namespace Booking_WEB.Controllers.API
{
    [Route("api/realty")]
    [ApiController]
    [AuthorizationFilter]
    public class RealtyController(
        ILogger<RealtyController> logger,
        RealtyAccessor realtyAccessor,
        IStorageService storageService,
        IOptions<StorageOptions> options) : ControllerBase
    {
        private readonly ILogger<RealtyController> _logger = logger;
        private readonly RealtyAccessor _realtyAccessor = realtyAccessor;
        private readonly IStorageService _storageService = storageService;
        IOptions<StorageOptions> _options = options;


        [HttpPost]
        public async Task<object> Create(CreateRealtyFormModel model)
        {
            string savedName;
            try
            {
                _storageService.TryGetMimeType(model.Image.FileName);
                savedName = await _storageService.SaveItemAsync(model.Image);

                var cityId = await _realtyAccessor.GetCityIdByNameAsync(model.City);
                var countryId = await _realtyAccessor.GetCountryIdByNameAsync(model.Country);
                var groupId = await _realtyAccessor.GetGroupIdByNameAsync(model.Group);

                if (model == null)
                {
                    return new
                    {
                        Status = 400,
                        Error = "Invalid JSON"
                    };
                }

                if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Slug))
                {
                    return new
                    {
                        Status = 400,
                        Error = "Name and Slug are required"
                    };
                }

                var exists = await _realtyAccessor.SlugExistsAsync(model.Slug);

                if (exists)
                {
                    return new
                    {
                        Status = 409,
                        Error = "Slug already exists"
                    };
                }

                var realty = new Realty
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Description = model.Description,
                    Slug = model.Slug,
                    ImageUrl = Path.Combine(_options.Value.StoragePath!, savedName),
                    Price = model.Price,
                    CityId = cityId,
                    CountryId = countryId,
                    GroupId = groupId,
                    DeletedAt = null
                };

                await _realtyAccessor.CreateAsync(realty);

                return new
                {
                    Status = 200,
                    Message = "Realty created",
                    Data = new
                    {
                        realty.Id,
                        realty.Name,
                        realty.Slug
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Realty/Create: {ex.Message}");
                return new
                {
                    Status = 500,
                    Error = ex.Message
                };
            }
        }

        [HttpPatch]
        public async Task<object> Update()
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
                    return new { Status = 400, Error = "Invalid input" };
                }

                var realty = await _realtyAccessor.GetByIdAsync(model.Id, true);

                if (realty == null)
                {
                    return (new { Status = 404, Error = "Realty not found" });
                }

                var slugExists = await _realtyAccessor.SlugExistsAsync(model.Slug!, model.Id);

                if (slugExists)
                {
                    return (new { Status = 409, Error = "Slug already exists" });
                }

                if (!string.IsNullOrEmpty(model.Name)) realty.Name = model.Name;
                if (!string.IsNullOrEmpty(model.Description)) realty.Description = model.Description;
                if (!string.IsNullOrEmpty(model.Slug)) realty.Slug = model.Slug;
                if (!string.IsNullOrEmpty(model.ImageUrl)) realty.ImageUrl = model.ImageUrl;

                if (model.Price > 0) realty.Price = model.Price;

                realty.CityId = model.CityId;
                realty.CountryId = model.CountryId;
                realty.GroupId = model.GroupId;

                await _realtyAccessor.UpdateAsync(realty);

                return (new
                {
                    Status = 200,
                    Message = "Realty updated",
                    Data = new { realty.Id }
                });
            }
            catch (Exception ex)
            {
                return (new { Status = 500, Error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<object> Delete()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                var json = JsonDocument.Parse(body);
                var idProp = json.RootElement.GetProperty("id");
                if (!Guid.TryParse(idProp.ToString(), out var id))
                {
                    return (new { Status = 400, Error = "Invalid ID" });
                }

                var realty = await _realtyAccessor.GetByIdAsync(id, true);

                if (realty == null)
                {
                    return (new { Status = 404, Error = "Realty not found" });
                }

                await _realtyAccessor.SoftDeleteAsync(realty);

                return (new
                {
                    Status = 200,
                    Message = "Realty deleted",
                    Data = new { realty.Id }
                });
            }
            catch (Exception ex)
            {
                return (new { Status = 500, Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<object> GetById(Guid id)
        {
            try
            {
                var realty = await _realtyAccessor.GetByIdAsync(id);
                if (realty == null)
                {
                    return new { Status = 404, Error = "Realty not found" };
                }
                return new
                {
                    Status = 200,
                    Data = realty
                };
            }
            catch (Exception ex)
            {
                return new { Status = 500, Error = ex.Message };
            }
        }

        [HttpGet("all")]
        public async Task<object> GetAll()
        {
            try
            {
                var realties = await _realtyAccessor.GetAllAsync();
                return new
                {
                    Status = 200,
                    Data = realties
                };
            }
            catch (Exception ex)
            {
                return new { Status = 500, Error = ex.Message };
            }
        }

        [HttpGet("filter")]
        public async Task<object> GetByFilter([FromQuery] string? country, [FromQuery] string? city, [FromQuery] string? group)
        {
            try
            {
                var realties = await _realtyAccessor.GetRealtiesByFilterAsync(country, city, group);
                return new
                {
                    Status = 200,
                    Data = realties
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Realty/GetByFilter: {ex.Message}");
                return new { Status = 500, Error = ex.Message };
            }
        }

        [HttpGet("sort/price")]
        public async Task<object> GetSortedByPrice()
        {
            try
            {
                var realties = await _realtyAccessor.GetRealtiesSortedByPrice();
                return new
                {
                    Status = 200,
                    Data = realties
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Realty/GetSortedByPrice: {ex.Message}");
                return new { Status = 500, Error = ex.Message };
            }
        }

        [HttpGet("sort/rating")]
        public async Task<object> GetSortedByRating()
        {
            try
            {
                var realties = await _realtyAccessor.GetRealtiesSortedByRating();
                return new
                {
                    Status = 200,
                    Data = realties
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Realty/GetSortedByRating: {ex.Message}");
                return new { Status = 500, Error = ex.Message };
            }
        }

    }
}
