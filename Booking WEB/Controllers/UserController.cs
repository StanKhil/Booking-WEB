using Booking_WEB.Data.Entities;
using Booking_WEB.Data;
using Booking_WEB.Models.User;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Booking_WEB.Services.Random;
using Booking_WEB.Services.Kdf;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Booking_WEB.Dto.User;
using Booking_WEB.Services.Time;
using Booking_WEB.Services.Jwt;
using System.Security.Claims;
using Booking_WEB.Data.DataAccessors;
using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace Booking_WEB.Controllers
{
    public class UserController(
        IRandomService randomService,
        IKdfService kdfService,
        ILogger<UserController> logger,
        UserAccessAccessor userAccessAccessor,
        UserDataAccessor userDataAccessor) : Controller
    {
        private readonly IRandomService _randomService = randomService;
        private readonly IKdfService _kdfService = kdfService;
        private readonly ILogger<UserController> _logger = logger;
        private readonly UserAccessAccessor _userAccessAccessor = userAccessAccessor;
        private readonly UserDataAccessor _userDataAccessor = userDataAccessor;
        const String authSessionKey = "userAccess";

        [HttpPost]
        public async Task<JsonResult> CreateAsync()
        {
            if (!IsAuthenticated())
                return Json(new { Status = 401, Error = "Unauthorized" });
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var model = JsonSerializer.Deserialize<UserDto>(body, options);

                if (model == null ||
                    string.IsNullOrWhiteSpace(model.FirstName) ||
                    string.IsNullOrWhiteSpace(model.LastName) ||
                    string.IsNullOrWhiteSpace(model.Email) ||
                    string.IsNullOrWhiteSpace(model.Login) ||
                    string.IsNullOrWhiteSpace(model.Password))
                {
                    return Json(new { Status = 400, Error = "FirstName, LastName, Email, Login and Password are required" });
                }

                if (await _userAccessAccessor.LoginExistsAsync(model.Login))
                {
                    return Json(new { Status = 409, Error = "Login already exists" });
                }

                var user = new UserData
                {
                    Id = Guid.NewGuid(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    BirthDate = model.BirthDate,
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

                return Json(new
                {
                    Status = 200,
                    Message = "User created",
                    Data = new
                    {
                        user.Id,
                        user.Email,
                        access.Login
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 500, Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateAsync()
        {
            if (!IsAuthenticated())
                return Json(new { Status = 401, Error = "Unauthorized" });
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var model = JsonSerializer.Deserialize<UserUpdateDto>(body, options);

                if (model == null || model.Id == Guid.Empty)
                {
                    return Json(new { Status = 400, Error = "Invalid input" });
                }

                var user = await _userDataAccessor.GetByIdAsync(model.Id);
                if (user == null)
                {
                    return Json(new { Status = 404, Error = "User not found" });
                }

                var access = user.UserAccesses.FirstOrDefault();
                if (access == null)
                {
                    return Json(new { Status = 500, Error = "User access not found" });
                }

                if (!string.IsNullOrWhiteSpace(model.Login) &&
                    await _userAccessAccessor.LoginExistsAsync(model.Login))
                {
                    return Json(new { Status = 409, Error = "Login already exists" });
                }

                if (!string.IsNullOrWhiteSpace(model.FirstName)) user.FirstName = model.FirstName;
                if (!string.IsNullOrWhiteSpace(model.LastName)) user.LastName = model.LastName;
                if (!string.IsNullOrWhiteSpace(model.Email)) user.Email = model.Email;
                if (model.BirthDate.HasValue) user.BirthDate = model.BirthDate;

                if (!string.IsNullOrWhiteSpace(model.Login)) access.Login = model.Login;
                if (!string.IsNullOrWhiteSpace(model.RoleId)) access.RoleId = model.RoleId;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    string salt = _randomService.Otp(12);
                    string dk = _kdfService.Dk(model.Password, salt);
                    access.Salt = salt;
                    access.Dk = dk;
                }

                await _userDataAccessor.SaveChangesAsync();

                return Json(new
                {
                    Status = 200,
                    Message = "User updated",
                    Data = new { user.Id }
                });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 500, Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteAsync()
        {
            if (!IsAuthenticated())
                return Json(new { Status = 401, Error = "Unauthorized" });
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                var json = JsonDocument.Parse(body);
                var userLogin = json.RootElement.GetProperty("login").GetString()!;

                bool loginExists = await _userAccessAccessor.LoginExistsAsync(userLogin);
                if (!loginExists)
                {
                    return Json(new { Status = 404, Error = "User not found" });
                }

                var deleted = await _userAccessAccessor.DeleteUserAsync(userLogin);

                if (!deleted)
                {
                    return Json(new { Status = 409, Error = "User deletion conflict" });
                }

                return Json(new
                {
                    Status = 200,
                    Message = "User deleted",
                    Data = new { Login = userLogin }
                });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 500, Error = ex.Message });
            }
        }


        // ============== PROFILE ==============
        public async Task<ViewResult> Profile(String id)
        {
            UserProfilePageModel model = new();

            var ua = await _userAccessAccessor.GerUserAccessByLoginAsync(id, isEditable: false);

            if (ua == null)
            {
                model.IsPersonal = null;
            }
            else
            {
                model.FirstName = ua.UserData.FirstName;
                model.LastName = ua.UserData.LastName;
                model.RegisteredAt = ua.UserData.RegisteredAt;

                bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
                if (isAuthenticated)
                {
                    model.Email = ua.UserData.Email;
                    String userLogin = HttpContext
                        .User
                        .Claims
                        .First(c => c.Type == ClaimTypes.Sid)
                        .Value;
                    if (ua.Login == userLogin)
                    {
                        model.IsPersonal = true;
                        model.Birthdate = ua.UserData.BirthDate;
                    }
                    else
                        model.IsPersonal = false;
                }
                else
                    model.IsPersonal = false;
            }
            return View(model);
        }

        public async Task<JsonResult> EditAsync() // In general ASP-project it is UpdateAsync
        {
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            if (!isAuthenticated)
            {
                return Json(new
                {
                    Status = 401,
                    Data = "Unauthorized"
                });
            }
            var userLogin = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Sid).Value;
            var userAccess = await _userAccessAccessor.GerUserAccessByLoginAsync(userLogin, isEditable: true);

            if (userAccess == null)
            {
                return Json(new
                {
                    Status = 403,
                    Data = "Forbidden"
                });
            }

            using StreamReader reader = new(Request.Body, Encoding.UTF8);
            var requestBody = await reader.ReadToEndAsync();
            if (requestBody == null)
            {
                return Json(new
                {
                    Status = 400,
                    Data = "Body must not be empty"
                });
            }
            JsonElement json;
            try
            {
                json = JsonSerializer.Deserialize<JsonElement>(requestBody);
            }
            catch (Exception e)
            {
                _logger.LogInformation("JSON decode error {e}", e.Message);
                return Json(new
                {
                    Status = 400,
                    Data = "Body must be a valid JSON string"
                });
            }
            if (json.ValueKind != JsonValueKind.Array)
            {
                return Json(new
                {
                    Status = 422,
                    Data = "Body must be a JSON array"
                });
            }
            foreach (var element in json.EnumerateArray())
            {
                string value = element.GetProperty("value").GetString()!;
                string field = element.GetProperty("field").GetString()!;
                switch (field)
                {
                    case "FirstName": userAccess.UserData.FirstName = value; break;
                    case "LastName": userAccess.UserData.LastName = value; break;
                    case "Email": userAccess.UserData.Email = value; break;
                    default:
                        return Json(new
                        {
                            Status = 409,
                            Data = $"Conflict: undefined field '{field}'"
                        });
                }
            }

            await _userAccessAccessor.UpdateUserAsync(userAccess);
            return Json(new
            {
                Status = 202,
                Data = "Accepted"
            });
        }

        [HttpDelete]
        public async Task<JsonResult> DeleteProfileAsync()
        {
            String authControl = HttpContext.Request.Headers["Authentication-Control"].ToString();
            if (String.IsNullOrEmpty(authControl))
            {
                return Json(new
                {
                    Status = 400,
                    Data = "Bad Request: Authentication-Control header is required"
                });
            }
            authControl = Encoding.UTF8.GetString(Base64UrlTextEncoder.Decode(authControl));

            
            if (!IsAuthenticated())
            {
                return Json(new
                {
                    Status = 401,
                    Data = "Unauthorized"
                });
            }
            String userLogin = HttpContext
                .User
                .Claims
                .First(c => c.Type == ClaimTypes.Sid)
                .Value;
            if (userLogin != authControl)
            {
                return Json(new
                {
                    Status = 403,
                    Data = "Forbidden: You can only delete your own account"
                });
            }

            bool isDeleted = await _userAccessAccessor.DeleteUserAsync(authControl);
            if (isDeleted)
            {
                HttpContext.Session.Remove(authSessionKey);
                Response.Cookies.Delete("AuthToken");
                return Json(new
                {
                    Status = 200,
                    Data = "User deleted successfully"
                });
            }
            else
            {
                return Json(new
                {
                    Status = 409,
                    Data = "User deletion conflict"
                });
            }
        }

        private bool IsAuthenticated()
        {
            return HttpContext.User.Identity?.IsAuthenticated ?? false;
        }
    }
}
