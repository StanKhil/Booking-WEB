using Booking_WEB.Data.Entities;
using Booking_WEB.Data;
using Booking_WEB.Models.User;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Booking_WEB.Services.Random;
using Booking_WEB.Services.Kdf;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Booking_WEB.Controllers
{
    public class UserController(IRandomService randomService, IKdfService kdfService, DataContext context, ILogger<UserController> logger) : Controller
    {
        private readonly IRandomService _randomService = randomService;
        private readonly IKdfService _kdfService = kdfService;
        private readonly DataContext _dataContext = context;
        private readonly ILogger<UserController> _logger = logger;
        private readonly Regex _passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!?@$&*])[A-Za-z\d@$!%*?&]{12,}$"); // For the time being

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
            HttpContext.Session.SetString(
                "UserSignupFormModel", JsonSerializer.Serialize(model)
            );
            return RedirectToAction(nameof(SignUp));
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
                _dataContext.Users.Add(user);
                _dataContext.UserAccesses.Add(userAccess);
                _dataContext.SaveChanges();

            }
            return errors;
        }

        //[HttpGet]
        //public ViewResult SignIn()
        //{
        //    UserSignInPageModel model = new();
        //    if (HttpContext.Session.Keys.Contains("UserSignInFormModel"))
        //    {
        //        model.FormModel = JsonSerializer.Deserialize<UserSignInFormModel>(
        //            HttpContext.Session.GetString("UserSignInFormModel")!);
        //        model.FormErrors = ProcessSignInData(model.FormModel!);

        //        HttpContext.Session.Remove("UserSignInFormModel");
        //    }

        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<RedirectToActionResult> SignIn(UserSignInFormModel model)
        //{
        //    HttpContext.Session.SetString("UserSignInFormModel",
        //        JsonSerializer.Serialize(model));
        //    return RedirectToAction(nameof(SignIn));
        //}

        [HttpGet]
        public ViewResult SignIn()
        {
            UserSignInPageModel model = new();

            if (HttpContext.Session.TryGetValue("UserSignInFormModel", out byte[]? formBytes))
            {
                var formJson = System.Text.Encoding.UTF8.GetString(formBytes);
                model.FormModel = JsonSerializer.Deserialize<UserSignInFormModel>(formJson);
                HttpContext.Session.Remove("UserSignInFormModel");
            }

            if (HttpContext.Session.TryGetValue("UserSignInFormErrors", out byte[]? errorBytes))
            {
                var errorsJson = System.Text.Encoding.UTF8.GetString(errorBytes);
                model.FormErrors = JsonSerializer.Deserialize<Dictionary<string, string>>(errorsJson);
                HttpContext.Session.Remove("UserSignInFormErrors");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult SignIn(UserSignInFormModel model)
        {
            var errors = ProcessSignInData(model);

            if (errors.Count > 0)
            {
                HttpContext.Session.SetString("UserSignInFormModel", JsonSerializer.Serialize(model));
                HttpContext.Session.SetString("UserSignInFormErrors", JsonSerializer.Serialize(errors));

                return RedirectToAction(nameof(SignIn));
            }

            return RedirectToAction("Index", "Home");
        }


        private Dictionary<string, string> ProcessSignInData(UserSignInFormModel model)
        {
            Dictionary<string, string> errors = [];

            if (string.IsNullOrWhiteSpace(model.UserLogin))
            {
                errors[nameof(model.UserLogin)] = "Login is required";
            }

            if (string.IsNullOrWhiteSpace(model.UserPassword))
            {
                errors[nameof(model.UserPassword)] = "Password is required";
            }

            if (errors.Count == 0)
            {
                var userAccess = _dataContext
                    .UserAccesses
                    .AsNoTracking()
                    .Include(ua => ua.UserData)
                    .Include(ua => ua.UserRole)
                    .FirstOrDefault(ua => ua.Login == model.UserLogin);

                if (userAccess == null || _kdfService.Dk(model.UserPassword, userAccess.Salt) != userAccess.Dk)
                {
                    errors["Authorization"] = "Invalid login or password";
                }
                else
                {
                    _logger.LogError("User {Login} signed in", userAccess.Login);
                    HttpContext.Session.SetString("userAccess",
                        JsonSerializer.Serialize(userAccess));
                }
            }

            return errors;
        }

    }
}
