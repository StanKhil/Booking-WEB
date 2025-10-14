using Booking_WEB.Data.DataAccessors;
using Booking_WEB.Data.Entities;
using Booking_WEB.Models.Rest;
using Booking_WEB.Models.User;
using Booking_WEB.Services.Jwt;
using Booking_WEB.Services.Kdf;
using Booking_WEB.Services.Random;
using Booking_WEB.Services.Time;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;



namespace Booking_WEB.Controllers.API
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(
        IRandomService randomService,
        IKdfService kdfService,
        ILogger<UserController> logger,
        ITimeService timeService,
        IJwtService jwtService,
        UserAccessAccessor userAccessAccessor,
        UserDataAccessor userDataAccessor,
        AccessTokenAccessor accessTokenAccessor) : Controller
    {

        private readonly IRandomService _randomService = randomService;
        private readonly IKdfService _kdfService = kdfService;
        private readonly ILogger<UserController> _logger = logger;
        private readonly Regex _passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!?@$&*])[A-Za-z\d@$!%*?&]{12,}$");
        private readonly ITimeService _timeService = timeService;
        private readonly IJwtService _jwtService = jwtService;
        private readonly UserAccessAccessor _userAccessAccessor = userAccessAccessor;
        private readonly UserDataAccessor _userDataAccessor = userDataAccessor;
        private readonly AccessTokenAccessor _accessTokenAccessor = accessTokenAccessor;
        const String authSessionKey = "userAccess";

        // ============= REGISTRATION ============= //
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserSignupFormModel model)
        {
            var errors = await ProcessSignupData(model);

            if (errors.Count > 0)
            {
                return BadRequest(new
                {
                    status = 400,
                    errors
                });
            }

            return Ok(new
            {
                status = 200,
                message = "Registration successful"
            });
        }

        private async Task<Dictionary<string, string>> ProcessSignupData(UserSignupFormModel model)
        {
            Dictionary<string, string> errors = new();

            // === Validation ===
            if (string.IsNullOrEmpty(model.UserFirstName))
                errors[nameof(model.UserFirstName)] = "First Name must not be empty!";
            if (string.IsNullOrEmpty(model.UserLastName))
                errors[nameof(model.UserLastName)] = "Last Name must not be empty!";
            if (string.IsNullOrEmpty(model.UserEmail))
                errors[nameof(model.UserEmail)] = "Email must not be empty!";
            if (string.IsNullOrEmpty(model.UserLogin))
                errors[nameof(model.UserLogin)] = "Login must not be empty!";
            else if (model.UserLogin.Contains(":"))
                errors[nameof(model.UserLogin)] = "Login must not contain ':'!";

            if (string.IsNullOrEmpty(model.UserPassword))
            {
                errors[nameof(model.UserPassword)] = "Password cannot be empty";
                errors[nameof(model.UserRepeat)] = "Invalid original password";
            }
            else if (!_passwordRegex.IsMatch(model.UserPassword))
            {
                errors[nameof(model.UserPassword)] =
                    "Password must be at least 12 characters long and contain lower, upper case letters, at least one number and at least one special character";
                errors[nameof(model.UserRepeat)] = "Invalid original password";
            }
            else if (model.UserRepeat != model.UserPassword)
            {
                errors[nameof(model.UserRepeat)] = "Passwords must match";
            }

            if (!model.Agree)
                errors[nameof(model.Agree)] = "You must agree with policies!";

            if (errors.Count > 0)
                return errors;

            // === Registration ===
            Guid userId = Guid.NewGuid();

            UserData user = new()
            {
                Id = userId,
                FirstName = model.UserFirstName,
                LastName = model.UserLastName,
                Email = model.UserEmail,
                BirthDate = model.Birthdate,
                RegisteredAt = DateTime.Now
            };

            string salt = _randomService.Otp(12);
            UserAccess userAccess = new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Login = model.UserLogin,
                Salt = salt,
                Dk = _kdfService.Dk(model.UserPassword, salt),
                RoleId = "SelfRegistered"
            };

            try
            {
                await _userDataAccessor.CreateAsync(user);
                await _userAccessAccessor.CreateAsync(userAccess);
            }
            catch (Exception e)
            {
                _logger.LogError("SignUp: {e}", e.Message);
                errors[nameof(model.UserLogin)] = "Login already exists";
            }

            return errors;
        }


        // ============= LOGIN =============
        private async Task<UserAccess> Authenticate()
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

            UserAccess? userAccess = await _userAccessAccessor.GerUserAccessByLoginAsync(login, false);

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
        public async Task<JsonResult> LogIn()
        {
            UserAccess userAccess;
            try
            {
                userAccess = await Authenticate();
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
                Exp = (_timeService.Timestamp() + 100).ToString(),
                Iss = nameof(Booking_WEB),
                Aud = userAccess.RoleId
            };

            await _accessTokenAccessor.CreateAsync(accessToken);

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
                RoleId = userAccess.UserRole.Id,
                userAccess.Login,
            };
            string jwt = _jwtService.EncodeJwt(jwtPayload);

            HttpContext.Session.SetString("AuthToken", jwt);
            HttpContext.Session.SetString("userAccess", JsonSerializer.Serialize(userAccess));

            return Json(new RestResponse
            {
                Status = RestStatus.RestStatus200,
                Data = jwt
            });
        }
    }
}
