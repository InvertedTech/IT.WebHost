using IT.WebHost.Core.Extensions;
using IT.WebHost.Core.Models;
using IT.WebServices.Authentication;
using IT.WebServices.Fragments;
using IT.WebServices.Fragments.Authentication;
using IT.WebServices.Fragments.Authorization.Payment;
using Microsoft.AspNetCore.Mvc;
using ProtoValidate;

namespace IT.WebHost.Admin.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserInterface.UserInterfaceClient _userClient;
        private readonly IT.WebHost.Core.Services.SiteSettingsService _siteSettings;
        private readonly PaymentInterface.PaymentInterfaceClient _paymentClient;
        private readonly ONUserHelper userHelper;
        private readonly IValidator _validator;

        public AuthController(UserInterface.UserInterfaceClient userClient, IT.WebHost.Core.Services.SiteSettingsService siteSettings, PaymentInterface.PaymentInterfaceClient paymentClient, ONUserHelper userHelper, IValidator validator)
        {
            _userClient = userClient;
            _siteSettings = siteSettings;
            _paymentClient = paymentClient;
            this.userHelper = userHelper;
            _validator = validator;
        }

        [HttpGet("/login")]
        public IActionResult Login()
        {
            ViewData["Title"] = $"Login - {_siteSettings.SiteTitle}";
            ViewData["Layout"] = "_LoginLayout";
            return View(new LoginViewModel());
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewData["Title"] = $"Login - {_siteSettings.SiteTitle}";
            ViewData["Layout"] = "_LoginLayout";

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

            var response = await _userClient.AuthenticateUserAsync(request);
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
    }
}
