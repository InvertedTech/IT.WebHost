using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IT.WebHost.Core.Authentication
{
    public class JwtCookieAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenValidationParameters _validationParameters;
        private readonly JwtSecurityTokenHandler _handler = new();

        public JwtCookieAuthenticationMiddleware(
            RequestDelegate next,
            TokenValidationParameters validationParameters)
        {
            _next = next;
            _validationParameters = validationParameters;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Cookies["token"];

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var principal = _handler.ValidateToken(
                        token,
                        _validationParameters,
                        out _);

                    context.User = principal;
                }
                catch
                {
                    // invalid token → leave user anonymous
                }
            }

            await _next(context);
        }
    }
}
