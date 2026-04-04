using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;
using IT.WebHost.Core.Clients;
using IT.WebServices.Fragments.Authentication;
using IT.WebServices.Fragments;
using IT.WebHost.Core.Models;
using IT.WebHost.App.Extensions;
using ProtoValidate;
using System.Diagnostics;
using IT.WebServices.Authentication;

namespace IT.WebHost.App.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthClient _authClient;
        private readonly IT.WebHost.Core.Services.SiteSettingsService _siteSettings;
        private readonly ONUserHelper userHelper;
        private readonly IAntiforgery _antiforgery;
        private readonly IValidator _validator;

        public AuthController(AuthClient authClient, IT.WebHost.Core.Services.SiteSettingsService siteSettings, ONUserHelper userHelper, IAntiforgery antiforgery, IValidator validator)
        {
            _authClient = authClient;
            _siteSettings = siteSettings;
            this.userHelper = userHelper;
            _antiforgery = antiforgery;
            _validator = validator;
        }

        [HttpGet("/login")]
        public IActionResult Login()
        {
            ViewData["Title"] = $"Login - {_siteSettings.SiteTitle}";
            ViewData["AntiforgeryToken"] = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken;
            return View(new LoginViewModel());
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewData["AntiforgeryToken"] = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken;
            ViewData["Title"] = $"Login - {_siteSettings.SiteTitle}";

            var request = new AuthenticateUserRequest
            {
                UserName = model.UserName ?? string.Empty,
                Password = model.Password ?? string.Empty,
                MFACode = string.Empty
            };

            var validated = _validator.Validate(request, false);
            if (!validated.IsSuccess)
            {
                ModelState.AddProtoViolations(validated);
                ViewData["UserNameErrorText"] = ModelState["UserName"]?.Errors.FirstOrDefault()?.ErrorMessage ?? string.Empty;
                ViewData["PasswordErrorText"] = ModelState["Password"]?.Errors.FirstOrDefault()?.ErrorMessage ?? string.Empty;
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _authClient.LoginAsync(request);
            if (response.Error.Reason == APIErrorReason.ErrorReasonNoError)
            {
                HttpContext.Response.Cookies.Append(JwtExtensions.JWT_COOKIE_NAME, response.BearerToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Path = "/"
                });

                return Redirect("/profile");
            }

            ViewData["LoginError"] = response.Error.Message;
            return View(model);
        }

        [HttpGet("/signup")]
        public IActionResult Signup()
        {
            ViewData["Title"] = $"Signup - {_siteSettings.SiteTitle}";
            ViewData["AntiforgeryToken"] = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken;
            return View(new SignupViewModel());
        }

        [HttpPost("/signup")]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            ViewData["AntiforgeryToken"] = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken;
            ViewData["Title"] = $"Signup - {_siteSettings.SiteTitle}";

            var request = new CreateUserRequest
            {
                UserName    = model.UserName    ?? string.Empty,
                DisplayName = model.DisplayName ?? string.Empty,
                Email       = model.Email       ?? string.Empty,
                Password    = model.Password    ?? string.Empty,
                FirstName   = model.FirstName   ?? string.Empty,
                LastName    = model.LastName    ?? string.Empty,
                PostalCode  = model.PostalCode  ?? string.Empty
            };

            var validated = _validator.Validate(request, false);
            if (!validated.IsSuccess)
            {
                ModelState.AddProtoViolations(validated);
                ViewData["UserNameErrorText"]    = ModelState["UserName"]?.Errors.FirstOrDefault()?.ErrorMessage    ?? string.Empty;
                ViewData["DisplayNameErrorText"] = ModelState["DisplayName"]?.Errors.FirstOrDefault()?.ErrorMessage ?? string.Empty;
                ViewData["EmailErrorText"]       = ModelState["Email"]?.Errors.FirstOrDefault()?.ErrorMessage       ?? string.Empty;
                ViewData["PasswordErrorText"]    = ModelState["Password"]?.Errors.FirstOrDefault()?.ErrorMessage    ?? string.Empty;
                ViewData["FirstNameErrorText"]   = ModelState["FirstName"]?.Errors.FirstOrDefault()?.ErrorMessage   ?? string.Empty;
                ViewData["LastNameErrorText"]    = ModelState["LastName"]?.Errors.FirstOrDefault()?.ErrorMessage    ?? string.Empty;
                ViewData["PostalCodeErrorText"]  = ModelState["PostalCode"]?.Errors.FirstOrDefault()?.ErrorMessage  ?? string.Empty;
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ViewData["ConfirmPasswordErrorText"] = "Passwords do not match";
                return View(model);
            }

            var response = await _authClient.SignUpAsync(request);
            if (response.Error.Reason == APIErrorReason.ErrorReasonNoError)
                return Redirect($"/auth/set-cookie?token={Uri.EscapeDataString(response.BearerToken)}");

            ViewData["SignupError"] = response.Error.Message;
            return View(model);
        }

        [HttpGet("/profile")]
        public async Task<IActionResult> Profile()
        {
            ViewData["Title"] = $"Profile - {_siteSettings.SiteTitle}";

            var response = await _authClient.GetOwnUserAsync();
            return View(new ProfileViewModel { UserRecord = response.Record != null ? ProfileData.FromRecord(response.Record) : null });
        }

        [HttpGet("/subscribe")]
        public IActionResult Subscribe()
        {
            ViewData["Title"] = $"Subscribe - {_siteSettings.SiteTitle}";
            var model = new SubscribeViewModel
            {
                Tiers = _siteSettings.Settings?.Subscription?.Tiers ?? new Google.Protobuf.Collections.RepeatedField<IT.WebServices.Fragments.Authorization.SubscriptionTier>()
            };
            return View(model);
        }
    }
}