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

namespace Booking_WEB.Controllers
{
    public class UserController(
        IRandomService randomService, 
        IKdfService kdfService, 
        DataContext context, 
        ILogger<UserController> logger) : Controller
    {
        private readonly IRandomService _randomService = randomService;
        private readonly IKdfService _kdfService = kdfService;
        private readonly DataContext _context = context;
        private readonly ILogger<UserController> _logger = logger;


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

                var model = JsonSerializer.Deserialize<UserDto>(body, options);

                if (model == null || string.IsNullOrWhiteSpace(model.FirstName) ||
                    string.IsNullOrWhiteSpace(model.LastName) || string.IsNullOrWhiteSpace(model.Email) ||
                    string.IsNullOrWhiteSpace(model.Login) || string.IsNullOrWhiteSpace(model.Password))
                {
                    return Json(new
                    {
                        Status = 400,
                        Error = "FirstName, LastName, Email, Login and Password are required"
                    });
                }

                var loginExists = await _context.UserAccesses
                    .AnyAsync(a => a.Login == model.Login);
                if (loginExists)
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
                    RegisteredAt = DateTime.UtcNow,
                };

                String salt = _randomService.Otp(12);
                var dk = _kdfService.Dk(model.Password, salt);

                var access = new UserAccess
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Login = model.Login,
                    Salt = salt,
                    Dk = dk,
                    RoleId = model.RoleId
                };

                user.UserAccesses.Add(access);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

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
                var model = JsonSerializer.Deserialize<UserUpdateDto>(body, options);

                if (model == null || model.Id == Guid.Empty)
                {
                    return Json(new { Status = 400, Error = "Invalid input" });
                }

                var user = await _context.Users
                    .Include(u => u.UserAccesses)
                    .FirstOrDefaultAsync(u => u.Id == model.Id && u.DeletedAt == null);

                if (user == null)
                {
                    return Json(new { Status = 404, Error = "User not found" });
                }

                var access = user.UserAccesses.FirstOrDefault();
                if (access == null)
                {
                    return Json(new { Status = 500, Error = "User access not found" });
                }

                var loginExists = await _context.UserAccesses
                    .AnyAsync(ua => ua.Login == model.Login && ua.UserId != model.Id);

                if (loginExists)
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

                await _context.SaveChangesAsync();

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

                var user = await _context.Users
                    .Include(u => u.UserAccesses)
                    .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);

                if (user == null)
                {
                    return Json(new { Status = 404, Error = "User not found" });
                }

                user.FirstName = String.Empty;
                user.LastName = String.Empty;
                user.Email = String.Empty;
                user.BirthDate = null;
                user.DeletedAt = DateTime.UtcNow;


                await _context.SaveChangesAsync();

                return Json(new
                {
                    Status = 200,
                    Message = "User deleted",
                    Data = new { user.Id }
                });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 500, Error = ex.Message });
            }
        }

        // ============== PROFILE ==============
        public ViewResult Profile(String id)
        {
            UserProfilePageModel model = new();
            var ua = _context
                .UserAccesses
                .AsNoTracking()
                .Include(ua => ua.UserData)
                .Include(ua => ua.UserRole)
                .FirstOrDefault(ua => ua.Login == id);
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
                    {
                        model.IsPersonal = false;
                    }
                }
                else
                {
                    model.IsPersonal = false;
                }
            }
            return View(model);
        }

    }
}
