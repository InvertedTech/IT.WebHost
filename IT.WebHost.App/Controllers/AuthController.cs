using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;
using IT.WebHost.Core.Clients;
using IT.WebServices.Fragments.Authentication;
using IT.WebServices.Fragments;
using IT.WebHost.Core.Models;
using System.Diagnostics;

namespace IT.WebHost.App.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthClient _authClient;
        private readonly IT.WebHost.Core.Services.SiteSettingsService _siteSettings;
        private readonly IAntiforgery _antiforgery;

        public AuthController(AuthClient authClient, IT.WebHost.Core.Services.SiteSettingsService siteSettings, IAntiforgery antiforgery)
        {
            _authClient = authClient;
            _siteSettings = siteSettings;
            _antiforgery = antiforgery;
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

            if (!ModelState.IsValid)
                return View(model);

            var request = new AuthenticateUserRequest
            {
                UserName = model.UserName,
                Password = model.Password,
                MFACode = string.Empty
            };

            var response = await _authClient.LoginAsync(request);
            if (response.Error.Reason == APIErrorReason.ErrorReasonNoError)
                return Redirect($"/auth/set-cookie?token={Uri.EscapeDataString(response.BearerToken)}");

            ViewData["LoginError"] = response.Error.Message;
            return View(model);
        }

        [HttpGet("/signup")]
        public IActionResult Signup()
        {
            ViewData["Title"] = $"Signup - {_siteSettings.SiteTitle}";
            return View(new SignupViewModel());
        }

        [HttpPost("/signup")]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = $"Signup - {_siteSettings.SiteTitle}";
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
                ViewData["Title"] = $"Signup - {_siteSettings.SiteTitle}";
                return View(model);
            }

            var request = new CreateUserRequest
            {
                UserName = model.UserName,
                DisplayName = model.DisplayName,
                Email = model.Email,
                Password = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PostalCode = model.PostalCode
            };

            var response = await _authClient.SignUpAsync(request);
            if (response.Error.Reason == APIErrorReason.ErrorReasonNoError)
            {
                return Redirect($"/auth/set-cookie?token={Uri.EscapeDataString(response.BearerToken)}");
            }
            else
            {
                ModelState.AddModelError("", response.Error.Message);
                ViewData["Title"] = $"Signup - {_siteSettings.SiteTitle}";
                return View(model);
            }
        }

        [HttpGet("/profile")]
        public async Task<IActionResult> Profile()
        {
            ViewData["Title"] = $"Profile - {_siteSettings.SiteTitle}";
            var token = Request.Cookies[AuthClient.CookieName];
            if (string.IsNullOrEmpty(token))
                return View(new ProfileViewModel());

            var response = await _authClient.GetOwnUserAsync(token);
            return View(new ProfileViewModel { UserRecord = response.Record });
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