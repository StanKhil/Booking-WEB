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
using Booking_WEB.Models.Rest;
using Azure;

namespace Booking_WEB.Controllers.API
{
    [Route("api/realty")]
    [ApiController]
    [AuthorizationFilter]
    public class RealtyController(
        ILogger<RealtyController> logger,
        RealtyAccessor realtyAccessor,
        IStorageService storageService,
        IOptions<StorageOptions> options,
        ItemImageAccessor itemImageAccessor) : ControllerBase
    {
        private readonly ILogger<RealtyController> _logger = logger;
        private readonly RealtyAccessor _realtyAccessor = realtyAccessor;
        private readonly IStorageService _storageService = storageService;
        private readonly ItemImageAccessor _itemImageAccessor = itemImageAccessor;
        IOptions<StorageOptions> _options = options;


        [HttpPost]
        public async Task<ActionResult<RestResponse>> Create([FromForm] CreateRealtyFormModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Slug))
                {
                    return BadRequest(new RestResponse
                    {
                        Status = RestStatus.RestStatus400,
                        Meta = BuildMeta("Create"),
                        Data = null
                    });
                }

                var exists = await _realtyAccessor.SlugExistsAsync(model.Slug);
                if (exists)
                {
                    return Conflict(new RestResponse
                    {
                        Status = new RestStatus { Code = 409, IsOk = false, Phrase = "Slug already exists" },
                        Meta = BuildMeta("Create"),
                        Data = null
                    });
                }

                _storageService.TryGetMimeType(model.Image.FileName);
                var savedName = await _storageService.SaveItemAsync(model.Image);

                var countryId = await _realtyAccessor.GetCountryIdByNameAsync(model.Country);
                var cityId = await _realtyAccessor.GetCityIdByNameAsync(model.City, countryId);
                var groupId = await _realtyAccessor.GetGroupIdByNameAsync(model.Group);

                var realty = new Realty
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Description = model.Description,
                    Slug = model.Slug,
                    Price = model.Price,
                    CityId = cityId,
                    GroupId = groupId,
                    DeletedAt = null
                };

                await _realtyAccessor.CreateAsync(realty);

                return CreatedAtAction(nameof(GetById), new { id = realty.Id }, new RestResponse
                {
                    Status = RestStatus.RestStatus201,
                    Meta = BuildMeta("Create"),
                    Data = new { realty.Id, realty.Name, realty.Slug }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Realty/Create: {ex.Message}");
                return StatusCode(500, new RestResponse
                {
                    Status = RestStatus.RestStatus500,
                    Meta = BuildMeta("Create"),
                    Data = ex.Message
                });
            }
        }


        [HttpPatch]
        public async Task<ActionResult<RestResponse>> Update([FromForm] UpdateRealtyFormModel model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new RestResponse
                    {
                        Status = RestStatus.RestStatus400,
                        Meta = BuildMeta("Update"),
                        Data = "Invalid input"
                    });
                }

                var realty = await _realtyAccessor.GetRealtyBySlugAsync(model.FormerSlug, true);

                if (realty == null)
                {
                    return NotFound(new RestResponse
                    {
                        Status = new RestStatus { Code = 404, IsOk = false, Phrase = "Realty not found" },
                        Meta = BuildMeta("Update"),
                        Data = null
                    });
                }

                var slugExists = await _realtyAccessor.SlugExistsAsync(model.Slug!, realty.Id);
                if (slugExists)
                {
                    return Conflict(new RestResponse
                    {
                        Status = new RestStatus { Code = 409, IsOk = false, Phrase = "Slug already exists" },
                        Meta = BuildMeta("Update"),
                        Data = null
                    });
                }

                if (!string.IsNullOrEmpty(model.Name)) realty.Name = model.Name;
                if (!string.IsNullOrEmpty(model.Description)) realty.Description = model.Description;
                if (!string.IsNullOrEmpty(model.Slug)) realty.Slug = model.Slug;
                if (model.Price > 0) realty.Price = model.Price;

                var countryId = await _realtyAccessor.GetCountryIdByNameAsync(model.Country!);
                var cityId = await _realtyAccessor.GetCityIdByNameAsync(model.City!, countryId);
                var groupId = await _realtyAccessor.GetGroupIdByNameAsync(model.Group!);

                realty.CityId = cityId;
                realty.GroupId = groupId;

                string? savedName = null;
                
                try
                {
                    await _realtyAccessor.DeleteImagesByRealtySlug(model.FormerSlug);
                    List<String> urls = new();

                    _storageService.TryGetMimeType(model.Image!.FileName);
                    savedName = await _storageService.SaveItemAsync(model.Image);
                    urls.Add(savedName);

                    foreach (var img in model.SecondaryImages!)
                    {
                        _storageService.TryGetMimeType(img.FileName);
                        savedName = await _storageService.SaveItemAsync(img);
                        urls.Add(savedName);
                    }

                    await _itemImageAccessor.AddRangeAsync(realty.Id, urls);
                }
                catch (Exception ex)
                {
                    return BadRequest(new RestResponse
                    {
                        Status = RestStatus.RestStatus400,
                        Meta = BuildMeta("Update"),
                        Data = $"Image upload failed: {ex.Message}"
                    });
                }
                

                await _realtyAccessor.UpdateAsync(realty);

                return Ok(new RestResponse
                {
                    Status = new RestStatus { Code = 200, IsOk = true, Phrase = "Updated" },
                    Meta = BuildMeta("Update"),
                    Data = new { realty.Id }
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


        [HttpDelete("{slug}")]
        public async Task<ActionResult<RestResponse>> DeleteBySlug(string slug)
        {
            try
            {
                var realty = await _realtyAccessor.GetRealtyBySlugAsync(slug, true);
                if (realty == null)
                {
                    return NotFound(new RestResponse
                    {
                        Status = new RestStatus { Code = 404, IsOk = false, Phrase = "Realty not found" },
                        Meta = BuildMeta("Delete"),
                        Data = null
                    });
                }

                await _realtyAccessor.SoftDeleteAsync(realty);

                return Ok(new RestResponse
                {
                    Status = new RestStatus { Code = 200, IsOk = true, Phrase = "Deleted" },
                    Meta = BuildMeta("Delete"),
                    Data = new { realty.Id }
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
                var realty = await _realtyAccessor.GetRealtyBySlugAsync(id.ToString());
                if (realty == null)
                {
                    return NotFound(new RestResponse
                    {
                        Status = new RestStatus { Code = 404, IsOk = false, Phrase = "Realty not found" },
                        Meta = BuildMeta("GetById"),
                        Data = null
                    });
                }

                return Ok(new RestResponse
                {
                    Status = new RestStatus { Code = 200, IsOk = true, Phrase = "OK" },
                    Meta = BuildMeta("GetById"),
                    Data = realty
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

        [HttpGet("all")]
        public async Task<ActionResult<RestResponse>> GetAll()
        {
            try
            {
                var realties = await _realtyAccessor.GetAllAsync();
                return Ok(new RestResponse
                {
                    Status = new RestStatus { Code = 200, IsOk = true, Phrase = "OK" },
                    Meta = BuildMeta("GetAll"),
                    Data = realties
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RestResponse
                {
                    Status = RestStatus.RestStatus500,
                    Meta = BuildMeta("GetAll"),
                    Data = ex.Message
                });
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult<RestResponse>> GetByFilter([FromQuery] string? country, [FromQuery] string? city, [FromQuery] string? group)
        {
            try
            {
                var realties = await _realtyAccessor.GetRealtiesByFilterAsync(country, city, group);
                return Ok(new RestResponse
                {
                    Status = new RestStatus { Code = 200, IsOk = true, Phrase = "OK" },
                    Meta = BuildMeta("GetByFilter"),
                    Data = realties
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Realty/GetByFilter: {ex.Message}");
                return StatusCode(500, new RestResponse
                {
                    Status = RestStatus.RestStatus500,
                    Meta = BuildMeta("GetByFilter"),
                    Data = ex.Message
                });
            }
        }


        [HttpGet("sort/price")]
        public async Task<ActionResult<RestResponse>> GetSortedByPrice()
        {
            try
            {
                var realties = await _realtyAccessor.GetRealtiesSortedByPrice();
                return Ok(new RestResponse
                {
                    Status = new RestStatus { Code = 200, IsOk = true, Phrase = "OK" },
                    Meta = BuildMeta("GetSortedByPrice"),
                    Data = realties
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RestResponse
                {
                    Status = RestStatus.RestStatus500,
                    Meta = BuildMeta("GetSortedByPrice"),
                    Data = ex.Message
                });
            }
        }

        [HttpGet("sort/rating")]
        public async Task<ActionResult<RestResponse>> GetSortedByRating()
        {
            try
            {
                var realties = await _realtyAccessor.GetRealtiesSortedByRating();
                return Ok(new RestResponse
                {
                    Status = new RestStatus { Code = 200, IsOk = true, Phrase = "OK" },
                    Meta = BuildMeta("GetSortedByRating"),
                    Data = realties
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RestResponse
                {
                    Status = RestStatus.RestStatus500,
                    Meta = BuildMeta("GetSortedByRating"),
                    Data = ex.Message
                });
            }
        }

        [HttpGet("filter/rate/{rate:int}")]
        public async Task<ActionResult<RestResponse>> GetByLowerRate(int rate)
        {
            try
            {
                var realties = await _realtyAccessor.GetRealtiesByLowerRate(rate);
                return Ok(new RestResponse
                {
                    Status = new RestStatus { Code = 200, IsOk = true, Phrase = "OK" },
                    Meta = BuildMeta("GetByLowerRate"),
                    Data = realties
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RestResponse
                {
                    Status = RestStatus.RestStatus500,
                    Meta = BuildMeta("GetByLowerRate"),
                    Data = ex.Message
                });
            }
        }

        [HttpGet("filter/price")]
        public async Task<ActionResult<RestResponse>> GetByPriceRange([FromQuery] decimal minPrice = 0, [FromQuery] decimal maxPrice = 10000)
        {
            try
            {
                var realties = await _realtyAccessor.GetRealtiesByPriceRange(minPrice, maxPrice);
                return Ok(new RestResponse
                {
                    Status = new RestStatus { Code = 200, IsOk = true, Phrase = "OK" },
                    Meta = BuildMeta("GetByPriceRange"),
                    Data = realties
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RestResponse
                {
                    Status = RestStatus.RestStatus500,
                    Meta = BuildMeta("GetByPriceRange"),
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
