using Microsoft.AspNetCore.Mvc;
using IT.WebHost.Core.Clients;
using IT.WebServices.Fragments.Authentication;
using IT.WebServices.Fragments;
using IT.WebHost.App.Models;
using System.Diagnostics;

namespace IT.WebHost.App.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthClient _authClient;
        private readonly IT.WebHost.Core.Services.SiteSettingsService _siteSettings;

        public AuthController(AuthClient authClient, IT.WebHost.Core.Services.SiteSettingsService siteSettings)
        {
            _authClient = authClient;
            _siteSettings = siteSettings;
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewData["Title"] = $"Login - {_siteSettings.SiteTitle}";
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = $"Login - {_siteSettings.SiteTitle}";
                return View(model);
            }

            var request = new AuthenticateUserRequest
            {
                UserName = model.UserName,
                Password = model.Password,
                MFACode = string.Empty
            };

            var response = await _authClient.LoginAsync(request);
            if (response.Error.Reason == APIErrorReason.ErrorReasonNoError)
            {
                return Redirect($"/auth/set-cookie?token={Uri.EscapeDataString(response.BearerToken)}");
            }
            else
            {
                ModelState.AddModelError("", response.Error.Message);
                ViewData["Title"] = $"Login - {_siteSettings.SiteTitle}";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Signup()
        {
            ViewData["Title"] = $"Signup - {_siteSettings.SiteTitle}";
            return View(new SignupViewModel());
        }

        [HttpPost]
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

        [HttpGet]
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