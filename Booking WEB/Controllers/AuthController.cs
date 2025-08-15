using Booking_WEB.Data.Entities;
using Booking_WEB.Models.User;
using Booking_WEB.Services.Random;
using Booking_WEB.Services.Time;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Booking_WEB.Dto.User;
using Booking_WEB.Services.Jwt;
using Booking_WEB.Data;
using Booking_WEB.Services.Kdf;
using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;

namespace Booking_WEB.Controllers
{
    public class AuthController(
        IRandomService randomService,
        IKdfService kdfService,
        DataContext context,
        ILogger<UserController> logger,
        ITimeService timeService,
        IJwtService jwtService) : Controller
    {

        private readonly IRandomService _randomService = randomService;
        private readonly IKdfService _kdfService = kdfService;
        private readonly DataContext _context = context;
        private readonly ILogger<UserController> _logger = logger;
        private readonly Regex _passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!?@$&*])[A-Za-z\d@$!%*?&]{12,}$");
        private readonly ITimeService _timeService = timeService;
        private readonly IJwtService _jwtService = jwtService;

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
            if (errors.Count > 0)
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
        private UserAccess Authenticate()
        {
            string authorizationHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                throw new Exception("Missing 'Authorization' header");
            }
            string authorizationScheme = "Basic ";
            if (!authorizationHeader.StartsWith(authorizationScheme))
            {
                throw new Exception($"Authorization scheme error: '{authorizationScheme}' only");
            }
            string credentials = authorizationHeader[authorizationScheme.Length..];
            string decoded;
            try
            {
                decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(credentials));
            }
            catch (Exception e)
            {
                _logger.LogError("SignIn: {e}", e.Message);
                throw new Exception($"Authorization credentials decode error");
            }
            string[] parts = decoded.Split(':', 2);
            if (parts.Length != 2)
            {
                throw new Exception($"Authorization credentials decompose error");
            }
            string login = parts[0];
            string password = parts[1];
            var userAccess = _context
                     .UserAccesses
                     .AsNoTracking()
                     .Include(ua => ua.UserData)
                     .Include(ua => ua.UserRole)
                     .FirstOrDefault(ua => ua.Login == login);
            if (userAccess == null)
            {
                throw new Exception($"Authorization credentials rejected: invalid login");
            }
            if (_kdfService.Dk(password, userAccess.Salt) != userAccess.Dk)
            {
                throw new Exception($"Authorization credentials rejected: invalid password");
            }
            return userAccess;
        }

        [HttpGet]
        public JsonResult LogIn()
        {
            UserAccess userAccess;
            try
            {
                userAccess = Authenticate();
            }
            catch (Exception e)
            {
                return Json(new
                {
                    Status = 401,
                    Data = e.Message
                });
            }

            AccessToken accessToken = new()
            {
                Jti = Guid.NewGuid().ToString(),
                Sub = userAccess.Id,
                Iat = _timeService.Timestamp().ToString(),
                Exp = (_timeService.Timestamp() + (long)1e1).ToString(),
                Iss = nameof(Booking_WEB),
                Aud = userAccess.RoleId
            };

            _context.AccessTokens.Add(accessToken);
            _context.SaveChanges();

            var jwtPayload = new
            {
                accessToken.Jti,
                accessToken.Sub,
                accessToken.Iat,
                accessToken.Exp,
                accessToken.Iss,
                accessToken.Aud,
                userAccess.UserData.FirstName,
                userAccess.UserData.LastName,
                userAccess.UserData.Email,
                userAccess.UserRole.Id,
                userAccess.Login,
            };
            string jwt = _jwtService.EncodeJwt(jwtPayload);
            Response.Cookies.Append("AuthToken", jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
            HttpContext.Session.SetString("userAccess", JsonSerializer.Serialize(userAccess));
            return Json(new
            {
                Status = 200,
                Data = _jwtService.EncodeJwt(jwt)
            });
        }

        public IActionResult LoginView()
        {
            if (HttpContext.Session.Keys.Contains("userAccess"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View("~/Views/User/LoginView.cshtml");
        }
    }
}
