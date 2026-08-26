using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly UserInterface.UserInterfaceClient userClient;
        private readonly ONUserHelper userHelper;

        public AuthController(UserInterface.UserInterfaceClient userClient, ONUserHelper userHelper)
        {
            this.userClient = userClient;
            this.userHelper = userHelper;
        }

        [HttpGet("refreshtoken")]
        public async Task<IActionResult> RefreshToken(string url)
        {
            var res = await userClient.RenewTokenAsync(new(), userHelper.GetGrpcCallOptions());
            var token = res?.BearerToken;
            if (string.IsNullOrEmpty(token))
            {
                return Redirect("/auth/logout");
            }

            Response.Cookies.Append(JwtExtensions.JWT_COOKIE_NAME, token, new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddDays(21),
                IsEssential = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });

            return Redirect(url);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete(JwtExtensions.JWT_COOKIE_NAME);
            return Redirect("/");
        }

        [HttpGet("set-cookie")]
        public async Task<IActionResult> SetCookie(string token, string? returnUrl)
        {
            Response.Cookies.Append(JwtExtensions.JWT_COOKIE_NAME, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddDays(21),
                IsEssential = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });

            return Redirect(returnUrl ?? "/");
        }
    }
}
