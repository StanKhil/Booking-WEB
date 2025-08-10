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

namespace Booking_WEB.Controllers
{
    public class UserController(IRandomService randomService, IKdfService kdfService, DataContext context, ILogger<UserController> logger) : Controller
    {
        private readonly IRandomService _randomService = randomService;
        private readonly IKdfService _kdfService = kdfService;
        private readonly DataContext _context = context;
        private readonly ILogger<UserController> _logger = logger;
        private readonly Regex _passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!?@$&*])[A-Za-z\d@$!%*?&]{12,}$"); // For the time being

        // ============= REGISTRATION ============= //
        public ViewResult SignUp()
        {
            UserSignupPageModel model = new();
            if (HttpContext.Session.Keys.Contains("UserSignupFormModel"))
            {
                model.FormModel = JsonSerializer.
                    Deserialize<UserSignupFormModel>(
                        HttpContext.Session.
                        GetString("UserSignupFormModel")!);
                model.FormErrors = ProcessSignupData(model.FormModel!);

                HttpContext.Session.Remove("UserSignupFormModel");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Register(UserSignupFormModel model)
        {
            var errors = ProcessSignupData(model);
            if(errors.Count > 0)
            {
                HttpContext.Session.SetString(
                    "UserSignupFormModel", JsonSerializer.Serialize(model)
                );
                return RedirectToAction(nameof(SignUp));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private Dictionary<String, String> ProcessSignupData(UserSignupFormModel model)
        {
            Dictionary<String, String> errors = [];
            #region Validation

            if (String.IsNullOrEmpty(model.UserFirstName)) errors[nameof(model.UserFirstName)] = "First Name must not be empty!";
            if (String.IsNullOrEmpty(model.UserLastName)) errors[nameof(model.UserLastName)] = "Last Name must not be empty!";
            if (String.IsNullOrEmpty(model.UserEmail)) errors[nameof(model.UserEmail)] = "Email must not be empty!";
            if (String.IsNullOrEmpty(model.UserLogin)) errors[nameof(model.UserLogin)] = "Login must not be empty!";
            else
            {
                if (model.UserLogin.Contains(":")) errors[nameof(model.UserLogin)] = "Login must not contain ':'!";
            }
            if (string.IsNullOrEmpty(model.UserPassword))
            {
                errors[nameof(model.UserPassword)] = "Password cannot be empty";
                errors[nameof(model.UserRepeat)] = "Invalid original password";
            }
            else
            {
                if (!_passwordRegex.IsMatch(model.UserPassword))
                {
                    errors[nameof(model.UserPassword)] = "Password must be at least 12 characters long and contain lower, upper case letters, at least one number and at least one special character";
                    errors[nameof(model.UserRepeat)] = "Invalid original password";
                }
                else
                {
                    if (model.UserRepeat != model.UserPassword) errors[nameof(model.UserRepeat)] = "Passwords must match";
                }
            }
            //if (String.IsNullOrEmpty(model.UserPassword)) errors[nameof(model.UserPassword)] = "Password must not be empty!";
            //else if (model.UserPassword.Length < 6) errors[nameof(model.UserPassword)] = "Password must be over 6 characters!";
            //else if (!model.UserPassword.Any(char.IsUpper))
            //    errors[nameof(model.UserPassword)] = "Password must contain at least one uppercase letter!";
            //
            //if (String.IsNullOrEmpty(model.UserRepeat))
            //    errors[nameof(model.UserRepeat)] = "Repeat password must not be empty!";
            //else if (model.UserRepeat != model.UserPassword)
            //    errors[nameof(model.UserRepeat)] = "Repeat password does not match!";

            if (!model.Agree) errors[nameof(model.Agree)] = "You must agree with policies!";

            #endregion
            if (errors.Count == 0)
            {
                Guid userId = Guid.NewGuid();

                UserData user = new()
                {
                    Id = userId,
                    FirstName = model.UserFirstName,
                    LastName = model.UserLastName,
                    Email = model.UserEmail,
                    BirthDate = model.Birthdate,
                    RegisteredAt = DateTime.Now,

                };

                String salt = _randomService.Otp(12);
                UserAccess userAccess = new()

                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Login = model.UserLogin,
                    Salt = salt,
                    Dk = _kdfService.Dk(model.UserPassword, salt),
                    RoleId = "SelfRegistered"
                };
                _context.Users.Add(user);
                _context.UserAccesses.Add(userAccess);
                _context.SaveChanges();

            }
            return errors;
        }


        // ============= LOGIN =============
        [HttpGet]
        public JsonResult SignIn()
        {
            string authorizationHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return Json(new { Status = 401, Data = "Missing 'Authorization' header" });
            }
            string authorizationScheme = "Basic ";
            if(!authorizationHeader.StartsWith(authorizationScheme))
            {
                return Json(new { Status = 401, Data = $"Authorization scheme error: '{authorizationScheme}' only" });
            }
            string credentials = authorizationHeader[authorizationScheme.Length..];
            string decoded;
            try
            {
                decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(credentials));
            }
            catch(Exception e)
            {
                _logger.LogError("SignIn: {e}", e.Message);
                return Json(new { Status = 401, Data = $"Authorization credentials decode error" });
            }
            string[] parts = decoded.Split(':', 2);
            if(parts.Length != 2)
            {
                return Json(new { Status = 401, Data = $"Authorization credentials decompose error" });
            }
            string login = parts[0];
            string password = parts[1];
            var userAccess = _context
                     .UserAccesses
                     .AsNoTracking()
                     .Include(ua => ua.UserData)
                     .Include(ua => ua.UserRole)
                     .FirstOrDefault(ua => ua.Login == login);
            if(userAccess == null)
            {
                return Json(new { Status = 401, Data = $"Authorization credentials rejected: invalid login" });
            }
            if(_kdfService.Dk(password, userAccess.Salt) != userAccess.Dk)
            {
                return Json(new { Status = 401, Data = $"Authorization credentials rejected: invalid password" });
            }
            HttpContext.Session.SetString("userAccess", JsonSerializer.Serialize(userAccess));
            return Json(new
            {
                Status = 200,
                Data = "Ok"
            });
        }


        public IActionResult Login()
        {
            //UserSignInPageModel model = new();
            //if (HttpContext.Session.Keys.Contains("UserSignInFormModel"))
            //{
            //    model.FormModel = JsonSerializer.Deserialize<UserSignInFormModel>(
            //        HttpContext.Session.GetString("UserSignInFormModel")!);
            //    model.FormErrors = ProcessSignInData(model.FormModel!);
            //
            //    HttpContext.Session.Remove("UserSignInFormModel");
            //}
            if(HttpContext.Session.Keys.Contains("userAccess"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //[HttpPost]
        //public async Task<RedirectToActionResult> SignIn(UserSignInFormModel model)
        //{
        //    HttpContext.Session.SetString("UserSignInFormModel",
        //        JsonSerializer.Serialize(model));
        //    return RedirectToAction(nameof(SignIn));
        //}

        //[HttpGet]
        //public ViewResult SignIn()
        //{
        //    UserSignInPageModel model = new();

        //    if (HttpContext.Session.TryGetValue("UserSignInFormModel", out byte[]? formBytes))
        //    {
        //        var formJson = System.Text.Encoding.UTF8.GetString(formBytes);
        //        model.FormModel = JsonSerializer.Deserialize<UserSignInFormModel>(formJson);
        //        HttpContext.Session.Remove("UserSignInFormModel");
        //    }

        //    if (HttpContext.Session.TryGetValue("UserSignInFormErrors", out byte[]? errorBytes))
        //    {
        //        var errorsJson = System.Text.Encoding.UTF8.GetString(errorBytes);
        //        model.FormErrors = JsonSerializer.Deserialize<Dictionary<string, string>>(errorsJson);
        //        HttpContext.Session.Remove("UserSignInFormErrors");
        //    }
        //    return View(model);
        //}

        //[HttpPost]
        //public IActionResult SignIn(UserSignInFormModel model)
        //{
        //    var errors = ProcessSignInData(model);

        //    if (errors.Count > 0)
        //    {
        //        HttpContext.Session.SetString("UserSignInFormModel", JsonSerializer.Serialize(model));
        //        HttpContext.Session.SetString("UserSignInFormErrors", JsonSerializer.Serialize(errors));

        //        return RedirectToAction(nameof(SignIn));
        //    }

        //    return RedirectToAction("Index", "Home");
        //}


        //private Dictionary<string, string> ProcessSignInData(UserSignInFormModel model)
        //{
        //    Dictionary<string, string> errors = [];

        //    if (string.IsNullOrWhiteSpace(model.UserLogin))
        //    {
        //        errors[nameof(model.UserLogin)] = "Login is required";
        //    }

        //    if (string.IsNullOrWhiteSpace(model.UserPassword))
        //    {
        //        errors[nameof(model.UserPassword)] = "Password is required";
        //    }

        //    if (errors.Count == 0)
        //    {
        //        var userAccess = _context
        //            .UserAccesses
        //            .AsNoTracking()
        //            .Include(ua => ua.UserData)
        //            .Include(ua => ua.UserRole)
        //            .FirstOrDefault(ua => ua.Login == model.UserLogin);

        //        if (userAccess == null || _kdfService.Dk(model.UserPassword, userAccess.Salt) != userAccess.Dk)
        //        {
        //            errors["Authorization"] = "Invalid login or password";
        //        }
        //        else
        //        {
        //            _logger.LogInformation("User {Login} signed in", userAccess.Login);
        //            HttpContext.Session.SetString("userAccess", JsonSerializer.Serialize(userAccess));
        //        }
        //    }
        //    return errors;
        //}

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

    }
}
