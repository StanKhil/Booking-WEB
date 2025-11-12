using Booking_WEB.Data.DataAccessors;
using Booking_WEB.Data.Entities;
using Booking_WEB.Models.Rest;
using Booking_WEB.Models.User;
using Booking_WEB.Services.Kdf;
using Booking_WEB.Services.Random;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking_WEB.Controllers.API
{
    [Route("api/user")]
    [ApiController]
    public class ApiUserController(
        UserAccessAccessor userAccessAccessor,
        UserDataAccessor userDataAccessor,
        IKdfService kdfService,
        IRandomService randomService
        ) : Controller
    {

        private readonly UserAccessAccessor _userAccessAccessor = userAccessAccessor;
        private readonly UserDataAccessor _userDataAccessor = userDataAccessor;
        private readonly IKdfService _kdfService = kdfService;
        private readonly IRandomService _randomService = randomService;
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] UserCreateFormModel model)
        {
            if (!IsAuthenticated())
            {
                return Unauthorized(new RestResponse
                {
                    Status = RestStatus.RestStatus401,
                    Meta = BuildMeta("User Creation"),
                    Data = "Unauthorized"
                });
            }
                

            if (model == null ||
                string.IsNullOrWhiteSpace(model.FirstName) ||
                string.IsNullOrWhiteSpace(model.LastName) ||
                string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Login) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest(new RestResponse
                {
                    Status = RestStatus.RestStatus400,
                    Meta = BuildMeta("User Creation"),
                    Data = "Invalid input"
                });
            }

            if (await _userAccessAccessor.LoginExistsAsync(model.Login))
            {
                return Conflict(new RestResponse
                {
                    Status = RestStatus.RestStatus409,
                    Meta = BuildMeta("User Creation"),
                    Data = "Login already exists"
                });
            }

            try
            {
                var user = new UserData
                {
                    Id = Guid.NewGuid(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    BirthDate = model.Birthdate,
                    RegisteredAt = DateTime.UtcNow
                };

                string salt = _randomService.Otp(12);
                string dk = _kdfService.Dk(model.Password, salt);

                var access = new UserAccess
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Login = model.Login,
                    Salt = salt,
                    Dk = dk,
                    RoleId = model.RoleId
                };

                await _userDataAccessor.CreateAsync(user);
                await _userAccessAccessor.CreateAsync(access);
                //return CreatedAtAction(nameof(GetByLogin), new { login = model.Login }, new
                //{
                //    user.Id,
                //    user.Email,
                //    access.Login
                //});
                return CreatedAtAction(nameof(GetByLogin), new { login = model.Login }, new RestResponse
                {
                    Status = RestStatus.RestStatus201,
                    Meta = BuildMeta("User Creation"),
                    Data = new
                    {
                        Message = "User created",
                        Data = new { user.Id, user.Email, access.Login }
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RestResponse
                {
                    Status = RestStatus.RestStatus500,
                    Meta = BuildMeta("User Creation"),
                    Data = $"Internal server error: {ex.Message}"
                });

            }
        }

        [HttpPatch("{login}")]
        public async Task<IActionResult> UpdateAsync(string login, [FromForm] UserUpdateDeleteModel model)
        {
            if (!IsAuthenticated())
                return Unauthorized(new RestResponse
                {
                    Status = RestStatus.RestStatus401,
                    Meta = BuildMeta("User Update"),
                    Data = "Unauthorized"
                });

            if (model == null)
                return BadRequest(new RestResponse
                {
                    Status = RestStatus.RestStatus400,
                    Meta = BuildMeta("User Update"),
                    Data = "Invalid input"
                });

            var userAccess = await _userAccessAccessor.GerUserAccessByLoginAsync(login, isEditable: true);
            if (userAccess == null)
                return NotFound(new RestResponse
                {
                    Status = RestStatus.RestStatus404,
                    Meta = BuildMeta("User Update"),
                    Data = "UserAccess not found"
                });

            var user = userAccess.UserData;
            if (user == null)
                return NotFound(new RestResponse
                {
                    Status = RestStatus.RestStatus404,
                    Meta = BuildMeta("User Update"),
                    Data = "User not found"
                });

            var access = user.UserAccesses.FirstOrDefault();
            if (access == null)
                return StatusCode(500, new RestResponse
                {
                    Status = RestStatus.RestStatus500,
                    Meta = BuildMeta("User Update"),
                    Data = "UserAccess data inconsistency"
                });

            if (!string.IsNullOrWhiteSpace(model.Login) &&
                model.Login != login &&
                await _userAccessAccessor.LoginExistsAsync(model.Login))
            {
                return Conflict(new RestResponse
                {
                    Status = RestStatus.RestStatus409,
                    Meta = BuildMeta("User Update"),
                    Data = "Login already exists"
                });
            }

            if (!string.IsNullOrWhiteSpace(model.FirstName)) user.FirstName = model.FirstName;
            if (!string.IsNullOrWhiteSpace(model.LastName)) user.LastName = model.LastName;
            if (!string.IsNullOrWhiteSpace(model.Email)) user.Email = model.Email;
            if (model.Birthdate.HasValue) user.BirthDate = model.Birthdate;

            if (!string.IsNullOrWhiteSpace(model.Login)) access.Login = model.Login;
            if (!string.IsNullOrWhiteSpace(model.RoleId)) access.RoleId = model.RoleId;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                string salt = _randomService.Otp(12);
                access.Salt = salt;
                access.Dk = _kdfService.Dk(model.Password, salt);
            }

            await _userDataAccessor.SaveChangesAsync();

            return Ok(new RestResponse
            {
                Status = RestStatus.RestStatus200,
                Meta = BuildMeta("User Update"),
                Data = "User updated successfully"
            });
        }


        [HttpDelete("{login}")]
        public async Task<IActionResult> DeleteAsync(string login)
        {
            if (!IsAuthenticated())
                return Unauthorized(new RestResponse
                {
                    Status = RestStatus.RestStatus401,
                    Meta = BuildMeta("User Deletion"),
                    Data = "Unauthorized"
                });

            bool loginExists = await _userAccessAccessor.LoginExistsAsync(login);
            if (!loginExists)
                return NotFound(new RestResponse
                {
                    Status = RestStatus.RestStatus404,
                    Meta = BuildMeta("User Deletion"),
                    Data = "User not found"
                });

            var deleted = await _userAccessAccessor.DeleteUserAsync(login);
            if (!deleted)
                return Conflict(new RestResponse
                {
                    Status = RestStatus.RestStatus409,
                    Meta = BuildMeta("User Deletion"),
                    Data = "Error deleting user"
                });

            return Ok(new RestResponse
            {
                Status = RestStatus.RestStatus200,
                Meta = BuildMeta("User Deletion"),
                Data = "User deleted successfully"
            });
        }

        private bool IsAuthenticated()
        {
            return HttpContext.User.Identity?.IsAuthenticated ?? false;
        }

        [HttpGet("{login}")]
        public async Task<IActionResult> GetByLogin(string login)
        {
            var ua = await _userAccessAccessor.GerUserAccessByLoginAsync(login);
            if (ua == null) return NotFound();

            return Json(new RestResponse
            {
                Data = new
                {
                    ua.UserData.Id,
                    ua.Login,
                    ua.UserData.FirstName,
                    ua.UserData.LastName,
                    ua.UserData.Email,
                    ua.Feedbacks,
                    ua.BookingItems
                },
                Status = RestStatus.RestStatus200,
            });
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
