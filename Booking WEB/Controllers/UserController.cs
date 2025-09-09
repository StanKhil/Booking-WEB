using Booking_WEB.Data.Entities;
using Booking_WEB.Data;
using Booking_WEB.Models.User;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Booking_WEB.Services.Random;
using Booking_WEB.Services.Kdf;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Booking_WEB.Services.Time;
using Booking_WEB.Services.Jwt;
using System.Security.Claims;
using Booking_WEB.Data.DataAccessors;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Booking_WEB.Controllers
{
    public class UserController(
        ILogger<UserController> logger,
        UserAccessAccessor userAccessAccessor,
        UserDataAccessor userDataAccessor) : Controller
    {
        private readonly ILogger<UserController> _logger = logger;
        private readonly UserAccessAccessor _userAccessAccessor = userAccessAccessor;
        private readonly UserDataAccessor _userDataAccessor = userDataAccessor;

        const String authSessionKey = "userAccess";

        // ============== PROFILE ==============
        public async Task<ViewResult> Profile([FromRoute]String id)
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
                        model.Cards = await _userDataAccessor.GetCardsByUserIdAsync(ua.UserId, isEditable: false);
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
            string authControl = HttpContext.Request.Headers["Authentication-Control"].ToString();
            if (string.IsNullOrEmpty(authControl))
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
            string userLogin = HttpContext
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
        public async Task<JsonResult> AddCardAsync()
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
            string? userLogin = HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value;
            UserAccess? userAccess = await _userAccessAccessor.GerUserAccessByLoginAsync(userLogin);
            string? role = userAccess?.RoleId;

            if (role != "Administrator")
            {
                return Json(new
                {
                    Status = 403,
                    Data = "Forbidden"
                });
            }

            Cards card = new Cards
            {
                Id = Guid.NewGuid(),
                UserId = userAccess!.UserData.Id
            };


            using StreamReader reader = new(Request.Body, Encoding.UTF8);
            var requestBody = await reader.ReadToEndAsync();
            if(requestBody == null)
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
            foreach(var element in json.EnumerateArray())
            {
                string value = element.GetProperty("value").GetString()!;
                string field = element.GetProperty("field").GetString()!;
                if(string.IsNullOrEmpty(value))
                {
                    return Json(new
                    {
                        Status = 400,
                        Data = "Bad request: " + field
                    });
                }
                switch (field)
                {
                    case "cardholder-name": card.CardholderName = value; break;
                    case "card-number": card.Number = value; break;
                    case "exp-date": card.ExpirationDate = new DateTime(Convert.ToInt32(value.Split('/')[1]), Convert.ToInt32(value.Split('/')[0]), 1); break;
                    default:
                        return Json(new
                        {
                            Status = 409,
                            Data = $"Conflict: undefined field '{field}'"
                        });
                }
            }
            await _userDataAccessor.CreateCardAsync(card);
            //_logger.LogTrace($"User card: {userAccess.UserData.Cards.First().Number}");
            return Json(new
            {
                Status = 202,
                Data = "Accepted"
            });
        }

        private bool IsAuthenticated()
        {
            return HttpContext.User.Identity?.IsAuthenticated ?? false;
        }
    }
}
