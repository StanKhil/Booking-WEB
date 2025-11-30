using Azure;
using Booking_WEB.Data;
using Booking_WEB.Data.DataAccessors;
using Booking_WEB.Data.Entities;
using Booking_WEB.Filters;
using Booking_WEB.Models.Realty;
using Booking_WEB.Models.Rest;
using Booking_WEB.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

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
        String imgPath => HttpContext.Request.Scheme + "://" +
               HttpContext.Request.Host + "/Storage/Item/";


        [HttpPost("search")]
        public async Task<ActionResult<RestResponse>> Search([FromBody] SearchFiltersModel filters)
        {
            try
            {
                List<Realty> rawRealties = await _realtyAccessor.GetRealtiesByFilterAsync(filters);
                IEnumerable<Realty> realties = AttachImagePaths(rawRealties);
                return Ok(new RestResponse
                {
                    Status = RestStatus.RestStatus200,
                    Meta = BuildMeta("Search"),
                    Data = realties
                });
            }
            catch (Exception e)
            {
                _logger.LogError($"BookingItem/Search: {e.Message}");
                return StatusCode(500, new RestResponse
                {
                    Status = RestStatus.RestStatus500,
                    Meta = BuildMeta("Search"),
                    Data = e.Message
                });
            }
        }


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
                await _itemImageAccessor.AddRangeAsync(realty.Id, new List<string> { savedName });
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

                realty = realty with
                {
                    Images = realty.Images?.Select(i => i with
                    {
                        ImageUrl = string.IsNullOrEmpty(i.ImageUrl) ? null : imgPath + i.ImageUrl
                    }).ToList() ?? [],

                    RealtyGroup = realty.RealtyGroup is null
                ? null!
                : realty.RealtyGroup with
                {
                    ImageUrl = string.IsNullOrEmpty(realty.RealtyGroup.ImageUrl)
                        ? null
                        : imgPath + realty.RealtyGroup.ImageUrl,

                    Images = realty.RealtyGroup.Images?.Select(gimg => gimg with
                    {
                        ImageUrl = string.IsNullOrEmpty(gimg.ImageUrl) ? null : imgPath + gimg.ImageUrl
                    }).ToList() ?? []
                }
                };
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
                var result = AttachImagePaths(realties);
                return Ok(new RestResponse
                {
                    Status = new RestStatus { Code = 200, IsOk = true, Phrase = "OK" },
                    Meta = BuildMeta("GetAll"),
                    Data = result
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

        [HttpGet("sort/price")]
        public async Task<ActionResult<RestResponse>> GetSortedByPrice()
        {
            try
            {
                var realties = await _realtyAccessor.GetRealtiesSortedByPrice();
                var result = AttachImagePaths(realties);
                return Ok(new RestResponse
                {
                    Status = new RestStatus { Code = 200, IsOk = true, Phrase = "OK" },
                    Meta = BuildMeta("GetSortedByPrice"),
                    Data = result
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
                var result = AttachImagePaths(realties);
                return Ok(new RestResponse
                {
                    Status = new RestStatus { Code = 200, IsOk = true, Phrase = "OK" },
                    Meta = BuildMeta("GetSortedByRating"),
                    Data = result
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
                var result = AttachImagePaths(realties);
                return Ok(new RestResponse
                {
                    Status = new RestStatus { Code = 200, IsOk = true, Phrase = "OK" },
                    Meta = BuildMeta("GetByLowerRate"),
                    Data = result
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
                var result = AttachImagePaths(realties);
                return Ok(new RestResponse
                {
                    Status = new RestStatus { Code = 200, IsOk = true, Phrase = "OK" },
                    Meta = BuildMeta("GetByPriceRange"),
                    Data = result
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

        private IEnumerable<Realty> AttachImagePaths(IEnumerable<Realty> realties)
        {
            return realties.Select(r => r with
            {
                Images = r.Images?.Select(img => img with
                {
                    ImageUrl = string.IsNullOrEmpty(img.ImageUrl) ? null : imgPath + img.ImageUrl
                }).ToList() ?? [],

                RealtyGroup = r.RealtyGroup is null ? null! : r.RealtyGroup with
                    {
                        ImageUrl = string.IsNullOrEmpty(r.RealtyGroup.ImageUrl)
                            ? null
                            : imgPath + r.RealtyGroup.ImageUrl,

                        Images = r.RealtyGroup.Images?.Select(gimg => gimg with
                        {
                            ImageUrl = string.IsNullOrEmpty(gimg.ImageUrl) ? null : imgPath + gimg.ImageUrl
                        }).ToList() ?? []
                    }
            });
        }

    }
}
